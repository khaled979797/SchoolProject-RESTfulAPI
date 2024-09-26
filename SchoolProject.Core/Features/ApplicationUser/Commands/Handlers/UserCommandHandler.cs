using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.ApplicationUser.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.ApplicationUser.Commands.Handlers
{
    public class UserCommandHandler : ResponseHandler,
        IRequestHandler<AddUserCommand, Response<string>>,
        IRequestHandler<EditUserCommand, Response<string>>,
        IRequestHandler<DeleteUserCommand, Response<string>>,
        IRequestHandler<ChangeUserPasswordCommand, Response<string>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IEmailService emailService;
        private readonly IApplicationUserService applicationUserService;
        #endregion

        #region Constructor
        public UserCommandHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IMapper mapper, UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor, IEmailService emailService,
            IApplicationUserService applicationUserService) : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.mapper = mapper;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.emailService = emailService;
            this.applicationUserService = applicationUserService;
        }
        #endregion

        #region Functions
        public async Task<Response<string>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            //Mapping
            var userMapper = mapper.Map<User>(request);

            //Create
            var createResult = await applicationUserService.AddUserAsync(userMapper, request.Password);

            switch (createResult)
            {
                case "EmailIsExist": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.EmailIsExist]);
                case "UserNameIsExist": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.UserNameIsExist]);
                //case "ErrorInCreateUser": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.FailedToAddUser]);
                case "Failed": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.TryToRegisterAgain]);
                case "Success": return Success(createResult);
                default: return BadRequest<string>(createResult);
            }
        }

        public async Task<Response<string>> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            //Check If Exist
            var user = await userManager.FindByIdAsync(request.Id.ToString());
            if (user == null) return NotFound<string>();
            //Mappin
            var userMapper = mapper.Map(request, user);

            //Check Username
            var userByUsername = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == userMapper.UserName & x.Id != userMapper.Id);
            if (userByUsername != null) return BadRequest<string>(stringLocalizer[SharedResourcesKeys.UserNameIsExist]);

            //Updating
            var result = await userManager.UpdateAsync(userMapper);
            //Result
            if (!result.Succeeded) return BadRequest<string>(stringLocalizer[SharedResourcesKeys.UpdateFailed]);
            return Success(stringLocalizer[SharedResourcesKeys.Updated].ToString());
        }

        public async Task<Response<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            //Check If Exist
            var user = await userManager.FindByIdAsync(request.Id.ToString());
            if (user == null) return NotFound<string>();
            //Deleting
            var createResult = await userManager.DeleteAsync(user);
            //Result
            if (!createResult.Succeeded) return BadRequest<string>(stringLocalizer[SharedResourcesKeys.DeletedFailed]);
            return Success(stringLocalizer[SharedResourcesKeys.Deleted].ToString());
        }

        public async Task<Response<string>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            //Check If Exist
            var user = await userManager.FindByIdAsync(request.Id.ToString());
            if (user == null) return NotFound<string>();
            //Check If Password Is Correct
            var checkPassword = await userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!checkPassword) return BadRequest<string>(stringLocalizer[SharedResourcesKeys.PasswordNotCorrect]);
            //Change Password
            var changeResult = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            //Return Result
            if (!changeResult.Succeeded) return BadRequest<string>(changeResult.Errors.FirstOrDefault().Description);
            return Success(stringLocalizer[SharedResourcesKeys.Success].ToString());
        }
        #endregion
    }
}
