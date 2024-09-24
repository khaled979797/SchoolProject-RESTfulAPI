using MediatR;
using SchoolProject.Core.Bases;
using SchoolProject.Data.Requests;

namespace SchoolProject.Core.Features.Authorization.Commands.Models
{
    public class EditUserClaimsCommand : EditUserClaimsRequest, IRequest<Response<string>>
    {
    }
}
