using Business;
using Business.DTO;
using Business.Migrations;
using Business.Model;
using DataAccess.IRepo;
using DataAccess.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repo
{
    public class UserRepo:IUserRepo
    {
        private readonly JWTService _jwtServices;
        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        private readonly EmailService _emailServices;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;

        //  private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepo(JWTService jwtServices, UserManager<User> userManager, SignInManager<User> signInManager, EmailService emailServices, IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            _jwtServices = jwtServices;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailServices = emailServices;
            _httpContextAccessor = httpContextAccessor;
            _context = context;

        }

        // Auth Methods
        //public async Task<string> RegisterAsync(RegisterDto registerDTO)
        //{

        //    if (registerDTO.Password != registerDTO.ConfirmPassword)
        //        throw new Exception("password and confirm password doesn't match");
        //    // Check if E-mail Exist
        //    var u = await _userManager.FindByEmailAsync(registerDTO.Email);
        //    if (u is not null) throw new Exception("The Email Already exist");


        //    var user = new User
        //    {
        //        Email = registerDTO.Email,
        //        PasswordHash = registerDTO.Password,
        //        UserName = registerDTO.Username,



        //    };
        //    var result = await _userManager.CreateAsync(user, registerDTO.Password);
        //    await _userManager.AddToRoleAsync(user, "user");
        //    //

        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        //    var confirmationLink = GenerateConfirmationLink(user.Id, token);


        //    _emailServices.SendEmail(user.Email, "SHIELD Email Confirmation",
        //   $"Please confirm your email by clicking here: <a href='{confirmationLink}'>Confirm</a>");
        //    //
        //    var errorMsg = result.Errors.FirstOrDefault()?.Description.ToString();
        //    if (!result.Succeeded)
        //        throw new Exception(errorMsg);

        //    return "Registration successful. Please check your email to confirm your account.";



        //}

        //public async Task<string> LoginAsync(LoginDto model)
        //{
        //    var user = await _userManager.FindByEmailAsync(model.Email);


        //    if (user is null) throw new UnauthorizedAccessException("Invalid Email or Password");

        //    if (await _userManager.IsLockedOutAsync(user))
        //    {
        //        throw new UnauthorizedAccessException("Your account has been locked. Please try again later.");
        //    }

        //    var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

        //    if (!result.Succeeded) throw new UnauthorizedAccessException("Invalid Email or Password");



        //    return await _jwtServices.GenerateJwtToken(user.Id);


        //}



        // User Profile Data


        public async Task<string> RegisterAsync(RegisterDto registerDTO)
        {
            if (registerDTO.Password != registerDTO.ConfirmPassword)
                throw new Exception("password and confirm password doesn't match");

            var u = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (u is not null) throw new Exception("The Email Already exist");

            var user = new User
            {
                Email = registerDTO.Email,
                PasswordHash = registerDTO.Password,
                UserName = registerDTO.Username,
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            await _userManager.AddToRoleAsync(user, "user");

            // Tạo OTP và gửi email
            var otp = new Random().Next(100000, 999999).ToString();
            user.SecurityStamp = otp;
            await _userManager.UpdateAsync(user);

            _emailServices.SendEmail(user.Email, "SHIELD OTP Verification",
               $"Your verification code is: <b>{otp}</b>");

            var errorMsg = result.Errors.FirstOrDefault()?.Description.ToString();
            if (!result.Succeeded)
                throw new Exception(errorMsg);

            return "Registration successful. Please check your email for the OTP to confirm your account.";
        }

        public async Task<string> ConfirmOTPAsync(string email, string inputOtp)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("User not found.");

            if (user.SecurityStamp != inputOtp)
                throw new Exception("Invalid OTP.");

            user.EmailConfirmed = true;
            user.SecurityStamp = Guid.NewGuid().ToString();
            await _userManager.UpdateAsync(user);

            return "Email confirmed successfully.";
        }

        public async Task<string> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null) throw new UnauthorizedAccessException("Invalid Email or Password");

            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedAccessException("Your account has been locked. Please try again later.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) throw new UnauthorizedAccessException("Invalid Email or Password");

            return await _jwtServices.GenerateJwtToken(user.Id);
        }

        // ... (Giữ nguyên các phương thức còn lại như cũ)
    

    public async Task<UserProfileDto> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new Exception("user Doesn't exist ");

            var userRoles = await _userManager.GetRolesAsync(user);

            var usrProfile = new UserProfileDto
            {
                Email = user.Email,
                Usernmae = user.UserName,
                roles = userRoles,
                Image = user.Image ?? "", // fallback nếu null
                PhoneNumber = user.PhoneNumber ?? "",
                Address = user.Address ?? "",
                IsPremium=user.IsPremium,
            };
            return usrProfile;



        }




        // User Email Confirm

        private string GenerateConfirmationLink(string userId, string token)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var confirmationLink = new Uri($"{request.Scheme}://{request.Host}/api/User/comfirmEmail?userId={userId}&token={Uri.EscapeDataString(token)}");
            return confirmationLink.ToString();
        }

        public async Task<string> ConfirmUserEmail(string userId, string Token)
        {
            if (userId == null || Token == null)
            {
                throw new BadHttpRequestException("Invalid Email Confirm Request");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new Exception("User Not found");

            var result = await _userManager.ConfirmEmailAsync(user, Token);
            var message = "Email Confirmed Successful";
            if (!result.Succeeded) throw new Exception("Email not Confirmed");
            return message;

        }

        private string GenerateEmailChangeConfirmationLink(string userId, string newEmail, string token)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var confirmationLink = new Uri($"{request.Scheme}://{request.Host}/api/User/ConfirmEmailChange?userId={userId}&newEmail={Uri.EscapeDataString(newEmail)}&token={Uri.EscapeDataString(token)}");
            return confirmationLink.ToString();
        }

        // User Chagne Settings

        public async Task<string> ChangeUserPassword(string userId, string currentPass, string newPass)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new Exception("User Not found");

            var result = await _userManager.ChangePasswordAsync(user, currentPass, newPass);
            if (!result.Succeeded)
            {
                var errorMsg = result.Errors.FirstOrDefault()?.Description ?? "Error changing password.";
                throw new Exception(errorMsg);
            }
            return "Password change successfuly";
        }
        public async Task<string> ChangeUserEmail(string userId, string newEmail)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new Exception("user Not Found");

            var emailExist = await _userManager.FindByEmailAsync(newEmail);
            if (emailExist != null)
                throw new Exception("The new Email already in use");

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

            var confirmationLink = GenerateEmailChangeConfirmationLink(user.Id, newEmail, token);

            _emailServices.SendEmail(newEmail, "Confirm your new email",
            $"Please confirm your new email by clicking here: <a href='{confirmationLink}'>link</a>");

            return "Email change requested. Please check your new email to confirm.";
        }

        public async Task<bool> ConfirmEmailChagen(string userId, string newEmail, string Token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            var result = await _userManager.ChangeEmailAsync(user, newEmail, Token);
            if (result.Succeeded)
            {
                await _userManager.SetUserNameAsync(user, newEmail); // Update the username if it's based on email
                return true;
            }

            return false;
        }


        
        public async Task<string> DeleteUserAccount(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user is null) throw new Exception("user not found");
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errorMsg = result.Errors.FirstOrDefault()?.Description ?? "Error deleting user.";
                throw new Exception(errorMsg);
            }
            return "Account Deleted Successfully";

        }

        public async Task<IEnumerable<object>> GetAllUser()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user); // Lấy danh sách roles của user
                userList.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.IsPremium,
                    Roles = roles,
                    user.LockoutEnd,
                    Image = user.Image ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    Address = user.Address ?? ""
                });
            }

            return userList;
        }

        public async Task<string> LockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

          var lockUser =  await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            if (!lockUser.Succeeded) {
                var errorMsg = lockUser.Errors.FirstOrDefault()?.Description ?? "Error locked user.";
                throw new Exception(errorMsg);
            }
            return "Account locked";
        }

        public async Task<string> UnLockUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

          var unLock =  await _userManager.SetLockoutEndDateAsync(user, null);
            await _userManager.ResetAccessFailedCountAsync(user);
            if (!unLock.Succeeded)
            {
                var errorMsg = unLock.Errors.FirstOrDefault()?.Description ?? "Error unlocked user.";
                throw new Exception(errorMsg);
            }
            return "Account unlocked";
        }

        public async Task<User> GetUserById(string userId)
        {
            var artifact = await _context.users.FirstOrDefaultAsync(o => o.Id == userId);
            if (artifact == null)
            {
                return null;
            }

            return artifact;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<string> GetUserRoleAsync(string userId)
        {
            var role = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                .FirstOrDefaultAsync();

            return role ?? "user";
        }

        public async Task<int> GetTotalUserCountAsync()
        {
            return await _userManager.Users.CountAsync();
        }

        public async Task<int> GetManagerCountAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("manager");
            return users.Count;
        }
        public async Task<int> GetUserCountAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("user");
            return users.Count;
        }
        public async Task<bool> UpdateUserMuseumAsync(string userId, int museumId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            // Optional: kiểm tra xem bảo tàng có tồn tại không
            var museumExists = await _context.museums.AnyAsync(m => m.Id == museumId);
            if (!museumExists)
                return false;

            user.MuseumId = museumId;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUserInforAsync(string userId, string phoneNumber, string image, string address)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.PhoneNumber = phoneNumber;
            user.Image = image;
            user.Address = address;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        
    }
}
