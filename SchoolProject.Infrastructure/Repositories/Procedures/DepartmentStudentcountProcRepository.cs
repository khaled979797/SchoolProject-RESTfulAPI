using SchoolProject.Data.Entities.Procedures;
using SchoolProject.Infrastructure.Abstracts.Procedures;
using SchoolProject.Infrastructure.Data;
using StoredProcedureEFCore;

namespace SchoolProject.Infrastructure.Repositories.Procedures
{
    public class DepartmentStudentcountProcRepository : IDepartmentStudentcountProcRepository
    {
        #region Fields
        private readonly AppDbContext context;
        #endregion

        #region Constructor
        public DepartmentStudentcountProcRepository(AppDbContext context)
        {
            this.context = context;
        }
        #endregion

        #region Functions
        public async Task<IReadOnlyCollection<DepartmentStudentcountProc>> GetDepartmentStudentcountProc(DepartmentStudentcountProcParameters parameters)
        {
            var rows = new List<DepartmentStudentcountProc>();
            await context.LoadStoredProc(nameof(DepartmentStudentcountProc))
                .AddParam(nameof(DepartmentStudentcountProcParameters.DID), parameters.DID)
                .ExecAsync(async r => rows = await r.ToListAsync<DepartmentStudentcountProc>());
            return rows;
        }
        #endregion
    }
}
