using MediatR;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Authorization.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Authorization.Commands.Handlers
{
    public class RoleCommandHandler : ResponseHandler,
        IRequestHandler<AddRoleCommand, Response<string>>,
        IRequestHandler<EditRoleCommand, Response<string>>,
        IRequestHandler<DeleteRoleCommand, Response<string>>,
        IRequestHandler<EditUserRolesCommand, Response<string>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IAuthorizationService authorizationService;

        #endregion

        #region Constructor
        public RoleCommandHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IAuthorizationService authorizationService) : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.authorizationService = authorizationService;
        }
        #endregion

        #region Functions
        public async Task<Response<string>> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await authorizationService.AddRoleAsync(request.RoleName);
            if (result == "Success") return Success("");
            return BadRequest<string>(stringLocalizer[SharedResourcesKeys.AddFailed]);
        }

        public async Task<Response<string>> Handle(EditRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await authorizationService.EditRoleAsync(request);
            if (result == "NotFound") return NotFound<string>();
            else if (result == "Success") return Success<string>(stringLocalizer[SharedResourcesKeys.Updated]);
            return BadRequest<string>(result);
        }

        public async Task<Response<string>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await authorizationService.DeleteRoleAsync(request.Id);
            if (result == "NotFound") return NotFound<string>();
            else if (result == "Used") return BadRequest<string>(stringLocalizer[SharedResourcesKeys.RoleIsUsed]);
            else if (result == "Success") return Success<string>(stringLocalizer[SharedResourcesKeys.Deleted]);
            return BadRequest<string>(result);
        }

        public async Task<Response<string>> Handle(EditUserRolesCommand request, CancellationToken cancellationToken)
        {
            var result = await authorizationService.UpdateUserRoles(request);
            switch (result)
            {
                case "UserNotFound": return NotFound<string>(stringLocalizer[SharedResourcesKeys.UserIsNotFound]);
                case "FailedToRemoveOldRoles": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.FailedToRemoveOldRoles]);
                case "FailedToAddNewRoles": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.FailedToAddNewRoles]);
                case "FailedToUpdateUserRoles": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.FailedToUpdateUserRoles]);
            }
            return Success(result);
        }
        #endregion
    }
}
