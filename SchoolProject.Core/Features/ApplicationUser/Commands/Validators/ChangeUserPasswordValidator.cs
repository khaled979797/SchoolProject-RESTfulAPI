using FluentValidation;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Features.ApplicationUser.Commands.Models;
using SchoolProject.Core.Resources;

namespace SchoolProject.Core.Features.ApplicationUser.Commands.Validators
{
    public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordCommand>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        #endregion

        #region Constructors
        public ChangeUserPasswordValidator(IStringLocalizer<SharedResources> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
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

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.Required]);

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.Required]);

            RuleFor(x => x.ConfirmNewPassword)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.Required])
                .Equal(x => x.NewPassword).WithMessage(stringLocalizer[SharedResourcesKeys.PasswordNotEqualConfirmPass]);
        }

        public void ApplyCustomValidationsRules()
        {

        }
        #endregion
    }
}
