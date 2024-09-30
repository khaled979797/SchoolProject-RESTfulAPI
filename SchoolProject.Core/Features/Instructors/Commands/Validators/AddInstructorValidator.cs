using FluentValidation;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Features.Instructors.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Instructors.Commands.Validators
{
    public class AddInstructorValidator : AbstractValidator<AddInstructorCommand>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IInstructorService instructorService;
        private readonly IDepartmentService departmentService;
        #endregion

        #region Constructors
        public AddInstructorValidator(IStringLocalizer<SharedResources> stringLocalizer,
            IInstructorService instructorService, IDepartmentService departmentService)
        {
            this.stringLocalizer = stringLocalizer;
            this.instructorService = instructorService;
            this.departmentService = departmentService;
            ApplyValidationsRules();
            ApplyCustomValidationsRules();
        }
        #endregion

        #region Functions
        public void ApplyValidationsRules()
        {
            RuleFor(x => x.ENameAr)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.Required]);

            RuleFor(x => x.ENameEn)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.Required]);

            RuleFor(x => x.DID)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.Required]);
        }

        public void ApplyCustomValidationsRules()
        {
            RuleFor(x => x.ENameAr)
                .MustAsync(async (Key, CancellationToken) => !await instructorService.IsNameArExist(Key))
                .WithMessage(stringLocalizer[SharedResourcesKeys.IsExist]);
            RuleFor(x => x.ENameEn)
                .MustAsync(async (Key, CancellationToken) => !await instructorService.IsNameEnExist(Key))
                .WithMessage(stringLocalizer[SharedResourcesKeys.IsExist]);


            RuleFor(x => x.DID)
            .MustAsync(async (Key, CancellationToken) => await departmentService.IsDepartmentIdExist(Key))
            .WithMessage(stringLocalizer[SharedResourcesKeys.IsNotExist]);
        }
        #endregion
    }
}
