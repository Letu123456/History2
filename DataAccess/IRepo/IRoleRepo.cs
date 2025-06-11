using Business.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.IRepo
{
    public interface IRoleRepo
    {
        Task<IEnumerable<Role>> GetRoles();

        Task<string> CreateRole(string Rolename);
        Task<string> DeleteRole(string roleId);


        // User Roles
        Task<IList<string>> GetUserRole(string userid);
        Task<string> AddUserToRole(string UserId, string RoleName);

        Task<string> RemoveUserFromRole(string UserId, string RoleName);

        Task<string> ChangeUserRole(string userId, string OldRoleName, string NewRoleNmae);
        Task<string> UpdateUserRole(string userId,  string NewRole);
    }
}
