using FluentValidation;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Features.Students.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Students.Commands.Validatiors
{
    public class EditStudentValidator : AbstractValidator<EditStudentCommand>
    {
        #region Fields
        private readonly IStudentService studentService;
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IDepartmentService departmentService;
        #endregion

        #region Constructors
        public EditStudentValidator(IStudentService studentService,
            IStringLocalizer<SharedResources> stringLocalizer, IDepartmentService departmentService)
        {
            this.studentService = studentService;
            this.stringLocalizer = stringLocalizer;
            this.departmentService = departmentService;
            ApplyValidationsRules();
            ApplyCustomValidationsRules();
        }
        #endregion

        #region Actions
        public void ApplyValidationsRules()
        {
            RuleFor(x => x.NameAr)
                 .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.NotEmpty])
                 .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.Required])
                 .MaximumLength(100).WithMessage(stringLocalizer[SharedResourcesKeys.MaxLengthis100]);

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.Required])
                .MaximumLength(100).WithMessage(stringLocalizer[SharedResourcesKeys.MaxLengthis100]);

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage(stringLocalizer[SharedResourcesKeys.NotEmpty])
                .NotNull().WithMessage(stringLocalizer[SharedResourcesKeys.Required]);
        }

        public void ApplyCustomValidationsRules()
        {
            RuleFor(x => x.NameAr)
                .MustAsync(async (model, Key, CancellationToken) => !await studentService.IsNameArExistExcludeSelf(Key, model.Id))
                .WithMessage(stringLocalizer[SharedResourcesKeys.IsExist]);

            RuleFor(x => x.NameEn)
                .MustAsync(async (model, Key, CancellationToken) => !await studentService.IsNameEnExistExcludeSelf(Key, model.Id))
                .WithMessage(stringLocalizer[SharedResourcesKeys.IsExist]);

            RuleFor(x => x.DepartmentId)
            .MustAsync(async (Key, CancellationToken) => await departmentService.IsDepartmentIdExist(Key))
            .WithMessage(stringLocalizer[SharedResourcesKeys.IsNotExist]);
        }

        #endregion
    }
}