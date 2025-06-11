using Business.Model;
using Business.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;

namespace DataAccess.Service
{
    public class GoogleService
    {
        private readonly UserManager<User> _userManager;
        private readonly JwtOptions _jwtOptions;

        public GoogleService(UserManager<User> userManager, JwtOptions jwtOptions)
        {
            _userManager = userManager;
            _jwtOptions = jwtOptions;
        }

        public async Task<GoogleLoginResult> LoginWithGoogleAsync(string idToken)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(idToken, new GoogleJsonWebSignature.ValidationSettings());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("StackTrace: " + ex.StackTrace);
                return null;
            }

            string email = payload.Email;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User { UserName = email, Email = email, Point = 0, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    return null;
                }
                var createdUser = await _userManager.FindByEmailAsync(email);
                if (createdUser == null)
                {
                    return null;
                }

                // Thêm role sau khi chắc chắn user đã tồn tại
                await _userManager.AddToRoleAsync(createdUser, "user");
            }

            var token = GenerateJwtToken(user);

            return new GoogleLoginResult
            {
                Token = token,
                UserId = user.Id,
                Username = user.Email
            };
        }

        private string GenerateJwtToken(User user)
        {
            // Lấy vai trò của người dùng
            var userRoles = _userManager.GetRolesAsync(user).Result; // Sử dụng Result vì không có async context

            // Tạo danh sách claims, đồng bộ với JWTService
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sid, user.Id),        // Thêm Sid
                new Claim(ClaimTypes.NameIdentifier, user.Id),          // Thêm NameIdentifier giống JWTService
                new Claim(JwtRegisteredClaimNames.Email, user.Email)    // Giữ lại Email
            };
            claims.AddRange(userRoles.Select(role => new Claim("role", role))); // Thêm vai trò

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtOptions.DurationInMinutes)), // Đồng bộ thời gian hết hạn
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public class GoogleLoginResult
        {
            public string Token { get; set; }
            public string UserId { get; set; }
            public string Username { get; set; }
        }
    }
}