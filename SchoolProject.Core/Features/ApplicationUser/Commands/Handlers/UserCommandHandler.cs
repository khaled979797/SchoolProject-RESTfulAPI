using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.ApplicationUser.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Data.Entities.Identity;

namespace SchoolProject.Core.Features.ApplicationUser.Commands.Handlers
{
    public class UserCommandHandler : ResponseHandler,
        IRequestHandler<AddUserCommand, Response<string>>,
        IRequestHandler<EditUserCommand, Response<string>>,
        IRequestHandler<DeleteUserCommand, Response<string>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        #endregion

        #region Constructor
        public UserCommandHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IMapper mapper, UserManager<User> userManager) : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.mapper = mapper;
            this.userManager = userManager;
        }
        #endregion

        #region Functions
        public async Task<Response<string>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            //Check Email
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user != null) return BadRequest<string>(stringLocalizer[SharedResourcesKeys.EmailIsExist]);

            //Check Username
            var userByUsername = await userManager.FindByNameAsync(request.UserName);
            if (userByUsername != null) return BadRequest<string>(stringLocalizer[SharedResourcesKeys.UserNameIsExist]);

            //Mapping
            var userMapper = mapper.Map<User>(request);

            //Create
            var createResult = await userManager.CreateAsync(userMapper, request.Password);

            //Failed
            if (!createResult.Succeeded)
            {
                return BadRequest<string>(createResult.Errors.FirstOrDefault().Description);
            }

            //Success
            return Created("");
        }

        public async Task<Response<string>> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            //Check If Exist
            var user = await userManager.FindByIdAsync(request.Id.ToString());
            if (user == null) return NotFound<string>();
            //Mappin
            var userMapper = mapper.Map(request, user);
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
        #endregion
    }
}
