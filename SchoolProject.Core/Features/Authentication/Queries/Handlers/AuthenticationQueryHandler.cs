using MediatR;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Authentication.Queries.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Authentication.Queries.Handlers
{
    public class AuthenticationQueryHandler : ResponseHandler,
        IRequestHandler<AuthorizeUserQuery, Response<string>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IAuthenticationService authenticationService;
        #endregion

        #region Constructor
        public AuthenticationQueryHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IAuthenticationService authenticationService) : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.authenticationService = authenticationService;
        }
        #endregion

        #region Functions
        public async Task<Response<string>> Handle(AuthorizeUserQuery request, CancellationToken cancellationToken)
        {
            var result = await authenticationService.ValidateToken(request.AccessToken);
            if (result == "NotExpired") return Success<string>(stringLocalizer[SharedResourcesKeys.TokenIsNotExpired]);
            return BadRequest<string>(stringLocalizer[SharedResourcesKeys.TokenIsExpired]);
        }

        #endregion
    }
}