using MediatR;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Authorization.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Authorization.Commands.Handlers
{
    public class ClaimCommandHandler : ResponseHandler,
        IRequestHandler<EditUserClaimsCommand, Response<string>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IAuthorizationService authorizationService;

        #endregion

        #region Constructor
        public ClaimCommandHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IAuthorizationService authorizationService) : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.authorizationService = authorizationService;
        }
        #endregion

        #region Functions
        public async Task<Response<string>> Handle(EditUserClaimsCommand request, CancellationToken cancellationToken)
        {
            var result = await authorizationService.UpdateUserClaims(request);
            switch (result)
            {
                case "UserNotFound": return NotFound<string>(stringLocalizer[SharedResourcesKeys.UserIsNotFound]);
                case "FailedToRemoveOldClaims": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.FailedToRemoveOldClaims]);
                case "FailedToAddNewClaims": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.FailedToAddNewClaims]);
                case "FailedToUpdateUserClaims": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.FailedToUpdateUserClaims]);
            }
            return Success(result);
        }
        #endregion
    }
}