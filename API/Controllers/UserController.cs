using Business.DTO;
using Business.Model;
using DataAccess.IRepo;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DataAccess.Service;
using System.Text.RegularExpressions;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepo _userRepo;
        private readonly GoogleService _googleService;
        private readonly UserManager<User> _userManager;
        private readonly IMuseumRepo _museumRepo;
        private readonly FilesService _filesService;

        public UserController(IUserRepo userRepo, GoogleService googleService, UserManager<User> userManager, IMuseumRepo museumRepo, FilesService filesService)
        {
            _userRepo = userRepo;
            _googleService = googleService;
            _userManager = userManager;
            _museumRepo = museumRepo;
            _filesService = filesService;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            try
            {
                return Ok(new { messege = await _userRepo.RegisterAsync(model) });
                
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
                //return BadRequest(new Dictionary<string, string> { { "message", ex.Message } });



            }


        }
        [HttpPost("confirm-otp")]
        public async Task<IActionResult> ConfirmOtp([FromBody] ComfirmOtpDtp model)
        {
            try
            {
                var message = await _userRepo.ConfirmOTPAsync(model.Email, model.Otp);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


       

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            try
            {
                // Gọi repo để xử lý đăng nhập, nhận token
                var token = await _userRepo.LoginAsync(model);

                // Nếu không có token, nghĩa là đăng nhập thất bại
                if (string.IsNullOrEmpty(token))
                {
                    return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
                }

                // Lấy user từ database
                var user = await _userRepo.GetUserByEmailAsync(model.Email);
                if (user == null)
                {
                    return Unauthorized(new { message = "Không tìm thấy người dùng." });
                }

                // Lấy vai trò (role) của user từ bảng userRoles & AspNetRoles
                var role = await _userRepo.GetUserRoleAsync(user.Id);

                if (string.IsNullOrEmpty(role))
                {
                    return Unauthorized(new { message = "Người dùng chưa có vai trò. Vui lòng liên hệ quản trị viên." });
                }

                // Trả về token và role của user
                return Ok(new { token = token, role = role ,id = user.Id});
               

    
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangeUserPassword(ChangePasswordDto model)
        {
            if (User is null) return Unauthorized();
            var userid = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
            if (userid is null) return Unauthorized();
            try
            {
                return Ok(new { message = await _userRepo.ChangeUserPassword(userid, model.CurrentPassword, model.NewPassword) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPut("ChangeEmail")]
        public async Task<IActionResult> ChnageUserEmail(ChangeEmailDto model)
        {
            if (User is null) return Unauthorized();
            var userid = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
            if (userid is null) return Unauthorized();
            try
            {
                return Ok(new { message = await _userRepo.ChangeUserEmail(userid, model.newEmail) });
            }
            catch (Exception ex)
            {

                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpDelete("DeleteAccount")]
        public async Task<IActionResult> DeleterUsrAccount(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { message = "User ID is required" });
            }

            try
            {
                return Ok(new { message = await _userRepo.DeleteUserAccount(userId) });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }


        }
        [HttpGet("Profile")]

        public async Task<IActionResult> GetUserProfile()
        {
            if (User is null) return Unauthorized();
            var userid = User.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
            if (userid is null) return Unauthorized();
            return Ok(await _userRepo.GetUserProfileAsync(userid));
        }

        [HttpGet("comfirmEmail")]
        public async Task<IActionResult> Confrim(string userId, string token)
        {
            try
            {
                return Ok(await _userRepo.ConfirmUserEmail(userId, token));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ConfirmEmailChange")]
        public async Task<IActionResult> ConfirmEmailChange(string userId, string newEmail, string token)
        {
            try
            {
                var result = await _userRepo.ConfirmEmailChagen(userId, newEmail, token);
                if (result)
                    return Ok("Email change confirmed successfully!");

                return BadRequest("Error confirming email change.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {

            try
            {
                return Ok(await _userRepo.GetAllUser());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetCountAllUser")]
        public async Task<IActionResult> GetCountAllUser()
        {

            try
            {
                return Ok(await _userRepo.GetTotalUserCountAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCountUser")]
        public async Task<IActionResult> GetCountUser()
        {

            try
            {
                return Ok(await _userRepo.GetUserCountAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetCountManager")]
        public async Task<IActionResult> GetCountManager()
        {

            try
            {
                return Ok(await _userRepo.GetManagerCountAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("lock/{userId}")]
        public async Task<IActionResult> LockUser(string userId)
        {

            if (userId == null)
            {
                return NotFound("userId null.");
            }

            try
            {
                return Ok(await _userRepo.LockUser(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("unlock/{userId}")]
        public async Task<IActionResult> UnLockUser(string userId)
        {

            if (userId == null)
            {
                return NotFound("userId null.");
            }

            try
            {
                return Ok(await _userRepo.UnLockUser(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var result = await _googleService.LoginWithGoogleAsync(request.IdToken);
            if (result == null)
            {
                return BadRequest("Google login failed.");
            }

            return Ok(new
            {
                token = result.Token,
                userId = result.UserId,
                username = result.Username
            });
        }

        [HttpPut("UpdateMuseumId")]
        public async Task<IActionResult> UpdateMuseumId(string userId, int museumId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new { error = "User ID is required." });

            if (museumId <= 0)
                return BadRequest(new { error = "Invalid Museum ID." });

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound(new { error = "User not found." });

                var museumExists = await _museumRepo.GetById(museumId);
                if (museumExists==null)
                    return NotFound(new { error = "Museum not found." });

                var result = await _userRepo.UpdateUserMuseumAsync(userId, museumId);
                return Ok(new { message = "Museum ID updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the Museum ID.", details = ex.Message });
            }
        }

        [HttpPut("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo( string? phoneNumber, IFormFile image, string? address)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            User user = await _userManager.FindByIdAsync(userId);
            if (user==null)
                return Unauthorized(new { error = "User is required." });

            string? uploadedImageUrl = null;

            // Upload ảnh lên S3 nếu có file
            if (image != null)
            {
                try
                {
                    uploadedImageUrl = await _filesService.UploadFileAsync(image, "");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to upload image: {ex.Message}");
                }
            }

            try
            {
                var isUpdated = await _userRepo.UpdateUserInforAsync(userId, phoneNumber, uploadedImageUrl, address);
                if (!isUpdated)
                    return NotFound(new { error = "User not found." });

                return Ok(new { message = "User information updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


    }

    public class GoogleLoginRequest
    {
        public string IdToken { get; set; }
    }
}