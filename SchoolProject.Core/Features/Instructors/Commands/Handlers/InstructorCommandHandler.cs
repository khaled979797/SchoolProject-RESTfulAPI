using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Instructors.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Data.Entities;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Instructors.Commands.Handlers
{
    public class InstructorCommandHandler : ResponseHandler,
        IRequestHandler<AddInstructorCommand, Response<string>>
    {
        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IInstructorService instructorService;
        private readonly IMapper mapper;
        #endregion

        #region Constructor
        public InstructorCommandHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IInstructorService instructorService, IMapper mapper) : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.instructorService = instructorService;
            this.mapper = mapper;
        }
        #endregion

        #region Functions
        public async Task<Response<string>> Handle(AddInstructorCommand request, CancellationToken cancellationToken)
        {
            var instructor = mapper.Map<Instructor>(request);
            var result = await instructorService.AddInstructorAsync(instructor, request.Image);
            switch (result)
            {
                case "NoImage": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.NoImage]);
                case "FailedToUpload": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.FailedToUploadImage]);
                case "FailedToAdd": return BadRequest<string>(stringLocalizer[SharedResourcesKeys.AddFailed]);
            }
            return Success(result);
        }
        #endregion
    }
}
