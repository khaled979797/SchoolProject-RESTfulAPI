using SchoolProject.Core.Features.ApplicationUser.Queries.Responses;
using SchoolProject.Data.Entities.Identity;

namespace SchoolProject.Core.Mapping.ApplicationUser
{
    public partial class ApplicationUserProfile
    {
        public void GetUserPaginatedListQueryMapping()
        {
            CreateMap<User, GetUserPaginatedListResponse>();
        }
    }
}
