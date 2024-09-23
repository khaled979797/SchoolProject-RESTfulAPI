using SchoolProject.Core.Features.Authorization.Queries.Responses;
using SchoolProject.Data.Entities.Identity;

namespace SchoolProject.Core.Mapping.Roles
{
    public partial class RoleProfile
    {
        public void GetRoleByIdQueryMapping()
        {
            CreateMap<Role, GetRoleByIdResponse>();
        }
    }
}
