using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Data.Helpers;
using SchoolProject.Data.Requests;
using SchoolProject.Data.Responses;
using SchoolProject.Infrastructure.Data;
using SchoolProject.Service.Abstracts;
using System.Security.Claims;

namespace SchoolProject.Service.Implementations
{
    public class AuthorizationService : IAuthorizationService
    {
        #region Fields
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        private readonly AppDbContext context;
        #endregion

        #region Constructor
        public AuthorizationService(RoleManager<Role> roleManager,
            UserManager<User> userManager, AppDbContext context)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.context = context;
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

        public async Task<List<Role>> GetRolesList()
        {
            return await roleManager.Roles.ToListAsync();
        }

        public async Task<Role> GetRoleById(int id)
        {
            return await roleManager.FindByIdAsync(id.ToString());
        }

        public async Task<ManageUserRolesResponse> ManageUserRolesData(User user)
        {
            var rolesList = new List<UserRole>();

            var roles = await roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var userRole = new UserRole
                {
                    Id = role.Id,
                    Name = role.Name,
                    HasRole = await userManager.IsInRoleAsync(user, role.Name) ? true : false
                };
                rolesList.Add(userRole);
            }

            return new ManageUserRolesResponse
            {
                UserId = user.Id,
                UserRoles = rolesList
            };
        }

        public async Task<string> UpdateUserRoles(EditUserRolesRequest request)
        {
            var transact = await context.Database.BeginTransactionAsync();
            try
            {
                var user = await userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null) return "UserNotFound";

                var userRoles = await userManager.GetRolesAsync(user);
                var removeOldRoles = await userManager.RemoveFromRolesAsync(user, userRoles);
                if (!removeOldRoles.Succeeded) return "FailedToRemoveOldRoles";

                var selectedRoles = request.UserRoles.Where(x => x.HasRole).Select(x => x.Name);
                var addNewRoles = await userManager.AddToRolesAsync(user, selectedRoles);
                if (!addNewRoles.Succeeded) return "FailedToAddNewRoles";
                await transact.CommitAsync();
                return "Success";
            }
            catch
            {
                await transact.RollbackAsync();
                return "FailedToUpdateUserRoles";
            }
        }

        public async Task<ManagerUserClaimsResponse> ManageUserClaimsData(User user)
        {
            var userClaimsList = new List<UserClaim>();

            var userClaims = await userManager.GetClaimsAsync(user);

            foreach (var claim in ClaimsStore.Claims)
            {
                var userClaim = new UserClaim
                {
                    Type = claim.Type,
                    Value = userClaims.Any(x => x.Type == claim.Type) ? true : false
                };
                userClaimsList.Add(userClaim);
            }
            return new ManagerUserClaimsResponse
            {
                UserId = user.Id,
                UserClaims = userClaimsList
            };
        }

        public async Task<string> UpdateUserClaims(EditUserClaimsRequest request)
        {
            var transact = await context.Database.BeginTransactionAsync();
            try
            {
                var user = await userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null) return "UserNotFound";

                var userClaims = await userManager.GetClaimsAsync(user);
                var removeOldClaims = await userManager.RemoveClaimsAsync(user, userClaims);
                if (!removeOldClaims.Succeeded) return "FailedToRemoveOldClaims";

                var selectedClaims = request.UserClaims.Where(x => x.Value).Select(x => new Claim(x.Type, x.Value.ToString()));
                var addNewClaims = await userManager.AddClaimsAsync(user, selectedClaims);
                if (!addNewClaims.Succeeded) return "FailedToAddNewClaims";
                await transact.CommitAsync();
                return "Success";
            }
            catch
            {
                await transact.RollbackAsync();
                return "FailedToUpdateUserClaims";
            }
        }
        #endregion
    }
}