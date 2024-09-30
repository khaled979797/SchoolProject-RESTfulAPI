using Microsoft.EntityFrameworkCore;
using SchoolProject.Data.Entities;
using SchoolProject.Data.Entities.Procedures;
using SchoolProject.Data.Entities.Views;
using SchoolProject.Infrastructure.Abstracts;
using SchoolProject.Infrastructure.Abstracts.Procedures;
using SchoolProject.Infrastructure.Abstracts.Views;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Service.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        #region Fields
        private readonly IDepartmentRepository departmentRepository;
        private readonly IViewRepository<ViewDepartment> viewDepartmentRepository;
        private readonly IDepartmentStudentcountProcRepository departmentStudentcountProc;
        #endregion

        #region Constructor
        public DepartmentService(IDepartmentRepository departmentRepository,
            IViewRepository<ViewDepartment> viewDepartmentRepository,
            IDepartmentStudentcountProcRepository departmentStudentcountProc)
        {
            this.departmentRepository = departmentRepository;
            this.viewDepartmentRepository = viewDepartmentRepository;
            this.departmentStudentcountProc = departmentStudentcountProc;
        }
        #endregion

        #region Functions
        public async Task<Department> GetDepartmentById(int id)
        {
            var department = await departmentRepository.GetTableNoTracking()
                .Where(x => x.DID.Equals(id))
                .Include(x => x.DepartmentSubjects).ThenInclude(x => x.Subject)
                .Include(x => x.Instructors).FirstOrDefaultAsync();

            return department;
        }

        public async Task<IReadOnlyCollection<DepartmentStudentcountProc>> GetDepartmentStudentcountProc(DepartmentStudentcountProcParameters parameters)
        {
            return await departmentStudentcountProc.GetDepartmentStudentcountProc(parameters);
        }

        public async Task<List<ViewDepartment>> GetViewDepartmentDataAsync()
        {
            return await viewDepartmentRepository.GetTableNoTracking().ToListAsync();
        }

        public async Task<bool> IsDepartmentIdExist(int id)
        {
            return await departmentRepository.GetTableNoTracking().AnyAsync(x => x.DID.Equals(id));
        }
        #endregion
    }
}