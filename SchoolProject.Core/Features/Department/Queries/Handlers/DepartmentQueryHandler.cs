using AutoMapper;
using MediatR;
using Microsoft.Extensions.Localization;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Department.Queries.Models;
using SchoolProject.Core.Features.Department.Queries.Responses;
using SchoolProject.Core.Resources;
using SchoolProject.Core.Wrappers;
using SchoolProject.Data.Entities;
using SchoolProject.Data.Entities.Procedures;
using SchoolProject.Service.Abstracts;
using System.Linq.Expressions;

namespace SchoolProject.Core.Features.Department.Queries.Handlers
{
    public class DepartmentQueryHandler : ResponseHandler,
        IRequestHandler<GetDepartmentByIdQuery, Response<GetDepartmentByIdResponse>>,
        IRequestHandler<GetDepartmentStudentListCountQuery, Response<List<GetDepartmentStudentListCountResponse>>>,
        IRequestHandler<GetDepartmentStudentCountByIdQuery, Response<GetDepartmentStudentCountByIdResponse>>
    {

        #region Fields
        private readonly IStringLocalizer<SharedResources> stringLocalizer;
        private readonly IDepartmentService departmentService;
        private readonly IMapper mapper;
        private readonly IStudentService studentService;
        #endregion
        #region Constructor
        public DepartmentQueryHandler(IStringLocalizer<SharedResources> stringLocalizer,
            IDepartmentService departmentService, IMapper mapper,
            IStudentService studentService) : base(stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
            this.departmentService = departmentService;
            this.mapper = mapper;
            this.studentService = studentService;
        }
        #endregion

        #region Functions
        public async Task<Response<GetDepartmentByIdResponse>> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
        {
            var response = await departmentService.GetDepartmentById(request.Id);
            if (response == null) return NotFound<GetDepartmentByIdResponse>(stringLocalizer[SharedResourcesKeys.NotFound]);
            var departmentMapper = mapper.Map<GetDepartmentByIdResponse>(response);

            Expression<Func<Student, StudentResponse>> expression = e => new StudentResponse(e.StudID, e.Localize(e.NameAr, e.NameEn));
            var studentQuerable = studentService.GetStudentsByDepartmentIdQuerable(request.Id);
            var studentPaginatedList = await studentQuerable.Select(expression).ToPaginatedListAsync(request.StudentPageNumber, request.StudentPageSize);
            departmentMapper.StudentList = studentPaginatedList;
            //Log.Information($"Get Department By Id {request.Id}");
            return Success(departmentMapper);
        }

        public async Task<Response<List<GetDepartmentStudentListCountResponse>>> Handle(GetDepartmentStudentListCountQuery request, CancellationToken cancellationToken)
        {
            var viewDepartments = await departmentService.GetViewDepartmentDataAsync();
            var viewDepartmentsMapper = mapper.Map<List<GetDepartmentStudentListCountResponse>>(viewDepartments);
            return Success(viewDepartmentsMapper);
        }

        public async Task<Response<GetDepartmentStudentCountByIdResponse>> Handle(GetDepartmentStudentCountByIdQuery request, CancellationToken cancellationToken)
        {
            var parameters = mapper.Map<DepartmentStudentcountProcParameters>(request);
            var procedureResult = await departmentService.GetDepartmentStudentcountProc(parameters);
            var result = mapper.Map<GetDepartmentStudentCountByIdResponse>(procedureResult.FirstOrDefault());
            return Success(result);
        }
        #endregion
    }
}
