using MediatR;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Authorization.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Authorization.Commands.Handlers
{
    public class RoleCommandHandler : ResponseHandler,
        IRequestHandler<AddRoleCommand, Response<string>>
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
        #endregion
    }
}
