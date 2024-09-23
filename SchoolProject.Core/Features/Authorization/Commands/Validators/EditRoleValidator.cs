using FluentValidation;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Features.Authorization.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Authorization.Commands.Validators
{
    public class EditRoleValidator : AbstractValidator<EditRoleCommand>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IAuthorizationService authorizationService;
        #endregion

        #region Constructors
        public EditRoleValidator(IStringLocalizer<SharedResources> stringLocalizer,
            IAuthorizationService authorizationService)
        {
            this.stringLocalizer = stringLocalizer;
            this.authorizationService = authorizationService;
            ApplyValidationsRules();
            ApplyCustomValidationsRules();
        }
        #endregion

        #region Functions
        public void ApplyValidationsRules()
        {
            RuleFor(x => x.Id)
                 .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.NotEmpty])
                 .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.Required]);

            RuleFor(x => x.Name)
                 .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.NotEmpty])
                 .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.Required]);
        }

        public void ApplyCustomValidationsRules()
        {
            RuleFor(x => x.Name)
                .MustAsync(async (Key, CancellationToken) => !await authorizationService.IsRoleExist(Key))
                .WithMessage(stringLocalizer[SharedResourcesKeys.RoleIsUsed]);
        }
        #endregion
    }
}
