using MediatR;
using SchoolProject.Core.Bases;
using SchoolProject.Data.Responses;

namespace SchoolProject.Core.Features.Authorization.Queries.Models
{
    public class ManageUserRolesQuery : IRequest<Response<ManageUserRolesResponse>>
    {
        public ManageUserRolesQuery(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; set; }
    }
}
