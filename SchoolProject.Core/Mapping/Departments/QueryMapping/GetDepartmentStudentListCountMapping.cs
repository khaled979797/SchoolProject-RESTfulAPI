using SchoolProject.Core.Features.Department.Queries.Responses;
using SchoolProject.Data.Entities.Views;

namespace SchoolProject.Core.Mapping.Departments
{
    public partial class DepartmentProfile
    {
        public void GetDepartmentStudentListCountMapping()
        {
            CreateMap<ViewDepartment, GetDepartmentStudentListCountResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Localize(src.DNameAr, src.DNameEn)));
        }
    }
}
