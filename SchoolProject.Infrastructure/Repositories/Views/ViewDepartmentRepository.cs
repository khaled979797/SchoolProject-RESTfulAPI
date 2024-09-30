using Microsoft.EntityFrameworkCore;
using SchoolProject.Data.Entities.Views;
using SchoolProject.Infrastructure.Abstracts.Views;
using SchoolProject.Infrastructure.Data;
using SchoolProject.Infrastructure.InfrastructureBases;

namespace SchoolProject.Infrastructure.Repositories.Views
{
    public class ViewDepartmentRepository : GenericRepositoryAsync<ViewDepartment>, IViewRepository<ViewDepartment>
    {
        #region Fields
        private readonly DbSet<ViewDepartment> viewDepartment;
        #endregion

        #region Constructor
        public ViewDepartmentRepository(AppDbContext context) : base(context)
        {
            viewDepartment = context.Set<ViewDepartment>();
        }
        #endregion

        #region Functions
        #endregion
    }
}
