using MediatR;
using SchoolProject.Core.Bases;
using SchoolProject.Data.Responses;

namespace SchoolProject.Core.Features.Authorization.Queries.Models
{
    public class ManageUserClaimsQuery : IRequest<Response<ManagerUserClaimsResponse>>
    {
        public ManageUserClaimsQuery(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; set; }
    }
}
