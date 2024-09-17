using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Students.Commands.Models;
using SchoolProject.Core.Resources;
using SchoolProject.Data.Entities;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Core.Features.Students.Commands.Handlers
{
    public class StudentCommandHandler : ResponseHandler,
        IRequestHandler<AddStudentCommand, Response<string>>,
        IRequestHandler<EditStudentCommand, Response<string>>,
        IRequestHandler<DeleteStudentCommand, Response<string>>
    {
        #region Fields
        private readonly IStudentService studentService;
        private readonly IMapper mapper;
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        #endregion

        #region Constructor
        public StudentCommandHandler(IStudentService studentService, IMapper mapper
            , IStringLocalizer<SharedResources> stringLocalizer) : base(stringLocalizer)
        {
            this.studentService = studentService;
            this.mapper = mapper;
            this.stringLocalizer = stringLocalizer;
        }
        #endregion

        #region Functions
        public async Task<Response<string>> Handle(AddStudentCommand request, CancellationToken cancellationToken)
        {
            //Mapping Between Request And Student
            var studentMapper = mapper.Map<Student>(request);
            //Adding
            var result = await studentService.AddAsync(studentMapper);
            //Return Response
            if (result == "Success") return Created("");
            else return BadRequest<string>();
        }

        public async Task<Response<string>> Handle(EditStudentCommand request, CancellationToken cancellationToken)
        {
            //Check If The Id Exist Or Not
            var student = await studentService.GetStudentByIdWithIncludeAsync(request.Id);
            //Return NotFound
            if (student == null) return NotFound<string>(stringLocalizer[SharedResourcesKeys.NotFound]);
            //Mapping Between Request And Student
            var studentMapper = mapper.Map(request, student);
            //Edit
            var result = await studentService.EditAsync(studentMapper);
            //Return Response
            if (result == "Success") return Success($"{stringLocalizer[SharedResourcesKeys.Updated]} {studentMapper.StudID}");
            else return BadRequest<string>();
        }

        public async Task<Response<string>> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            //Check If The Id Exist Or Not
            var student = await studentService.GetByIdAsync(request.Id);
            //Return NotFound
            if (student == null) return NotFound<string>(stringLocalizer[SharedResourcesKeys.NotFound]);
            //Call Service  That Makes The Delete
            var result = await studentService.DeleteAsync(student);
            if (result == "Success") return Deleted<string>($"{stringLocalizer[SharedResourcesKeys.Deleted]} {request.Id}");
            else return BadRequest<string>();
        }
        #endregion
    }
}
