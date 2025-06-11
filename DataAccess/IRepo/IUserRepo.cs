using Business.DTO;
using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IUserRepo
    {
        Task<string> RegisterAsync(RegisterDto registerDTO);
        Task<string> LoginAsync(LoginDto user);
        Task<IEnumerable<object>> GetAllUser();
        // User Profile Data
        Task<UserProfileDto> GetUserProfileAsync(string userId);
        Task<User> GetUserById(string userId);
        // User Email Confirm
        Task<string> ConfirmUserEmail(string userId, string Token);

        // Change User settings
        Task<string> ChangeUserPassword(string userId, string currentPass, string newPass);
        Task<string> ChangeUserEmail(string userId, string newEmail);
        Task<string> DeleteUserAccount(string UserId);
        Task<bool> ConfirmEmailChagen(string userId, string newEmail, string Toekn);

        Task<string> LockUser(string userId);
        Task<string> UnLockUser(string userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<string> GetUserRoleAsync(string userId);
        Task<int> GetTotalUserCountAsync();
        Task<int> GetManagerCountAsync();
        Task<int> GetUserCountAsync();
        Task<bool> UpdateUserMuseumAsync(string userId,int museumId);
        Task<bool> UpdateUserInforAsync(string userId, string phoneNumber,string image,string address);
        Task<string> ConfirmOTPAsync(string email, string otp);


    }
}
