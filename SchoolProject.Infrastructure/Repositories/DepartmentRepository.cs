using Microsoft.EntityFrameworkCore;
using SchoolProject.Data.Entities;
using SchoolProject.Infrastructure.Abstracts;
using SchoolProject.Infrastructure.Data;
using SchoolProject.Infrastructure.InfrastructureBases;

namespace SchoolProject.Infrastructure.Repositories
{
    public class DepartmentRepository : GenericRepositoryAsync<Department>, IDepartmentRepository
    {
        #region Fields
        private readonly DbSet<Department> deparments;
        #endregion

        #region Constructor
        public DepartmentRepository(AppDbContext context) : base(context)
        {
            deparments = context.Set<Department>();
        }
        #endregion

        #region Functions
        #endregion
    }
}
