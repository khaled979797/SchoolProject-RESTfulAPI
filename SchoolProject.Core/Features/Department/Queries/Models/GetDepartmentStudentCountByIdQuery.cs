using MediatR;
using SchoolProject.Core.Bases;
using SchoolProject.Core.Features.Department.Queries.Responses;

namespace SchoolProject.Core.Features.Department.Queries.Models
{
    public class GetDepartmentStudentCountByIdQuery : IRequest<Response<GetDepartmentStudentCountByIdResponse>>
    {
        public GetDepartmentStudentCountByIdQuery(int dID)
        {
            DID = dID;
        }

        public int DID { get; set; }
    }
}
