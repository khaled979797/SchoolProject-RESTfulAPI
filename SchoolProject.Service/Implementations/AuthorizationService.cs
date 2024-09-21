using Microsoft.AspNetCore.Identity;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Service.Implementations
{
    public class AuthorizationService : IAuthorizationService
    {
        #region Fields
        private readonly RoleManager<Role> roleManager;
        #endregion

        #region Constructor
        public AuthorizationService(RoleManager<Role> roleManager)
        {
            this.roleManager = roleManager;
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

        public async Task<bool> IsRoleExist(string roleName)
        {
            return await roleManager.RoleExistsAsync(roleName);
        }
        #endregion
    }
}