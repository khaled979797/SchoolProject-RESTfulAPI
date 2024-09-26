using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Authentication.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Data.Responses;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Authentication.Commands.Handlers
{
    public class AuthenticationCommandHandler : ResponseHandler,
        IRequestHandler<SignInCommand, Response<JwtAuthResponse>>,
        IRequestHandler<RefreshTokenCommand, Response<JwtAuthResponse>>,
        IRequestHandler<SendResetPasswordCommand, Response<string>>,
        IRequestHandler<ResetPasswordCommand, Response<string>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly IAuthenticationService authenticationService;
        #endregion

        #region Constructor
        public AuthenticationCommandHandler(IStringLocalizer<SharedResources> stringLocalizer,
            UserManager<User> userManager, SignInManager<User> signInManager,
            IAuthenticationService authenticationService) : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.authenticationService = authenticationService;
        }
        #endregion

        #region Functions
        public async Task<Response<JwtAuthResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            //Check If User Exist
            var user = await userManager.FindByNameAsync(request.UserName);
            if (user == null) return NotFound<JwtAuthResponse>(stringLocalizer[SharedResourcesKeys.UserNameIsNotExist]);

            //Check If Password Correct
            var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (!signInResult.Succeeded) return BadRequest<JwtAuthResponse>(stringLocalizer[SharedResourcesKeys.PasswordNotCorrect]);

            //Check Confirm Email
            if (!user.EmailConfirmed) return BadRequest<JwtAuthResponse>(stringLocalizer[SharedResourcesKeys.EmailNotConfirmed]);

            //Generate Token
            var result = await authenticationService.GetJwtToken(user);
            return Success(result);
        }

        public async Task<Response<JwtAuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var jwtToken = authenticationService.ReadJwtToken(request.AccessToken);
            var userIdAndExpireDate = await authenticationService.ValidateDetails(jwtToken, request.AccessToken, request.RefreshToken);
            switch (userIdAndExpireDate)
            {
                case ("AlgorithmIsWrong", null): return Unauthorized<JwtAuthResponse>(stringLocalizer[SharedResourcesKeys.AlgorithmIsWrong]);
                case ("TokenIsNotExpired", null): return Unauthorized<JwtAuthResponse>(stringLocalizer[SharedResourcesKeys.TokenIsNotExpired]);
                case ("RefreshTokenIsNotFound", null): return Unauthorized<JwtAuthResponse>(stringLocalizer[SharedResourcesKeys.RefreshTokenIsNotFound]);
                case ("RefreshTokenIsExpired", null): return Unauthorized<JwtAuthResponse>(stringLocalizer[SharedResourcesKeys.RefreshTokenIsExpired]);
            }

            var (userId, expireDate) = userIdAndExpireDate;
            var user = await userManager.FindByIdAsync(userId);
            if (user == null) return NotFound<JwtAuthResponse>();
            var result = await authenticationService.GetRefreshToken(user, jwtToken, request.RefreshToken, expireDate);
            return Success(result);
        }

        public async Task<Response<string>> Handle(SendResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await authenticationService.SendResetPasswordCode(request.Email);
            switch (result)
            {
                case "UserNotFound": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.UserIsNotFound]);
                case "ErrorInUpdateUser": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.TryAgainInAnotherTime]);
                case "Failed": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.TryAgainInAnotherTime]);
                case "Success": return Success(result);
                default: return BadRequest<string>(stringLocalizer[SharedResourcesKeys.TryAgainInAnotherTime]);
            }
        }

        public async Task<Response<string>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await authenticationService.ResetPassword(request.Email, request.Password);
            switch (result)
            {
                case "UserNotFound": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.UserIsNotFound]);
                case "Failed": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.TryAgainInAnotherTime]);
                case "Success": return Success(result);
                default: return BadRequest<string>(stringLocalizer[SharedResourcesKeys.TryAgainInAnotherTime]);
            }
        }
        #endregion
    }
}