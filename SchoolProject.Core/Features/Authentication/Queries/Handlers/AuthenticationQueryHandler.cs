using MediatR;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Authentication.Queries.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Authentication.Queries.Handlers
{
    public class AuthenticationQueryHandler : ResponseHandler,
        IRequestHandler<AuthorizeUserQuery, Response<string>>,
        IRequestHandler<ConfirmEmailQuery, Response<string>>,
        IRequestHandler<ConfirmResetPasswordQuery, Response<string>>
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

        public async Task<Response<string>> Handle(ConfirmEmailQuery request, CancellationToken cancellationToken)
        {
            var confirmEmail = await authenticationService.ConfirmEmail(request.UserId, request.Code);
            if (confirmEmail == "ErrorWhenConfirmEmail") return BadRequest<string>(stringLocalizer[SharedResourcesKeys.ErrorWhenConfirmEmail]);
            return Success<string>(stringLocalizer[SharedResourcesKeys.ConfirmEmailDone]);
        }

        public async Task<Response<string>> Handle(ConfirmResetPasswordQuery request, CancellationToken cancellationToken)
        {
            var result = await authenticationService.ConfirmResetPassword(request.Code, request.Email);
            switch (result)
            {
                case "UserNotFound": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.UserIsNotFound]);
                case "Failed": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.InvaildCode]);
                case "Success": return Success(result);
                default: return BadRequest<string>(stringLocalizer[SharedResourcesKeys.InvaildCode]);
            }
        }

        #endregion
    }
}