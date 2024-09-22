using Microsoft.AspNetCore.Identity;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Data.Requests;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Service.Implementations
{
    public class AuthorizationService : IAuthorizationService
    {
        #region Fields
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        #endregion

        #region Constructor
        public AuthorizationService(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        #endregion

        #region Functions

        public async Task<string> AddRoleAsync(string roleName)
        {
            var identityRole = new Role();
            identityRole.Name = roleName;
            var result = await roleManager.CreateAsync(identityRole);
            if (result.Succeeded) return "Success";
            return "Failed";
        }

        public async Task<bool> IsRoleExistByName(string roleName)
        {
            return await roleManager.RoleExistsAsync(roleName);
        }

        public async Task<string> EditRoleAsync(EditRoleRequest editRoleRequest)
        {
            var role = await roleManager.FindByIdAsync(editRoleRequest.Id.ToString());
            if (role == null) return "NotFound";
            role.Name = editRoleRequest.Name;
            var result = await roleManager.UpdateAsync(role);
            if (result.Succeeded) return "Success";
            var errors = String.Join("-", result.Errors);
            return errors;
        }

        public async Task<string> DeleteRoleAsync(int roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            if (role == null) return "NotFound";

            var users = await userManager.GetUsersInRoleAsync(role.Name);
            if (users != null && users.Count() > 0) return "Used";

            var result = await roleManager.DeleteAsync(role);
            if (result.Succeeded) return "Success";
            var errors = String.Join("-", result.Errors);
            return errors;
        }
        #endregion
    }
}