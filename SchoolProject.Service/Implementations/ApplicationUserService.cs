using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Infrastructure.Data;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Service.Implementations
{
    public class ApplicationUserService : IApplicationUserService
    {
        #region Fields
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AppDbContext context;
        private readonly IUrlHelper urlHelper;
        private readonly IEmailService emailService;
        #endregion

        #region Constructor
        public ApplicationUserService(UserManager<User> userManager, IEmailService emailService,
            IHttpContextAccessor httpContextAccessor, AppDbContext context, IUrlHelper urlHelper)
        {
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
            this.urlHelper = urlHelper;
            this.emailService = emailService;
        }
        #endregion

        #region Functions
        public async Task<string> AddUserAsync(User user, string password)
        {
            var transact = await context.Database.BeginTransactionAsync();
            try
            {
                //Check Email
                var existUser = await userManager.FindByEmailAsync(user.Email);
                if (existUser != null) return "EmailIsExist";

                //Check Username
                var userByUsername = await userManager.FindByNameAsync(user.UserName);
                if (userByUsername != null) return "UserNameIsExist";

                //Create
                var createResult = await userManager.CreateAsync(user, password);

                //Failed
                if (!createResult.Succeeded) return string.Join(",", createResult.Errors.Select(x => x.Description).ToList());

                await userManager.AddToRoleAsync(user, "User");

                //Send Confirm Email
                var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var requestAccessor = httpContextAccessor.HttpContext.Request;
                var returnUrl = requestAccessor.Scheme + "://" + requestAccessor.Host + urlHelper.Action("ConfirmEmail", "Authentication", new { userId = user.Id, code = code });
                var message = $"To Confirm Email Click The Link: <a href='{returnUrl}'></a>";
                //Message
                var sendEmailResult = await emailService.SendEmail(user.Email, message, "Confirm Email");
                await transact.CommitAsync();
                return "Success";
            }
            catch
            {
                await transact.RollbackAsync();
                return "Failed";
            }
        }
        #endregion
    }
}
