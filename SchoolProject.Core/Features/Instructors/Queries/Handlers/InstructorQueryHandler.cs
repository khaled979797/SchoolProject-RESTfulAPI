using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Instructors.Queries.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Instructors.Queries.Handlers
{
    public class InstructorQueryHandler : ResponseHandler,
        IRequestHandler<GetSummationSalaryOfInstructorQuery, Response<decimal>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IMapper mapper;
        private readonly IInstructorService instructorService;
        #endregion

        #region Constructor
        public InstructorQueryHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IMapper mapper, IInstructorService instructorService) : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.mapper = mapper;
            this.instructorService = instructorService;
        }
        #endregion

        #region Functions
        public async Task<Response<decimal>> Handle(GetSummationSalaryOfInstructorQuery request, CancellationToken cancellationToken)
        {
            var result = await instructorService.GetSalarySummationOfInstructor();
            return Success(result);
        }
        #endregion
    }
}
