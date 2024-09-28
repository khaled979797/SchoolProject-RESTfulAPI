using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Data.Helpers;
using SchoolProject.Service.AuthServices.Interfaces;

namespace SchoolProject.Service.AuthServices.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        #region Fields
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<User> userManager;
        #endregion

        #region Constructor
        public CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }
        #endregion

        #region Functions
        public async Task<User> GetUserAsync()
        {
            var user = await userManager.FindByIdAsync(GetUserId().ToString());
            if (user == null) throw new UnauthorizedAccessException();
            return user;
        }

        public int GetUserId()
        {
            var userId = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == nameof(UserClaimModel.Id)).Value;
            if (userId == null) throw new UnauthorizedAccessException();
            return int.Parse(userId);
        }

        public async Task<List<string>> GetCurrentUserRolesAsync()
        {
            var roles = await userManager.GetRolesAsync(await GetUserAsync());
            return roles.ToList();
        }
        #endregion
    }
}
