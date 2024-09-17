using Microsoft.EntityFrameworkCore;
using SchoolProject.Data.Entities;
using SchoolProject.Infrastructure.Abstracts;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Service.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        #region Fields
        private readonly IDepartmentRepository departmentRepository;
        #endregion

        #region Constructor
        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            this.departmentRepository = departmentRepository;
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

        public async Task<bool> IsDepartmentIdExist(int id)
        {
            return await departmentRepository.GetTableNoTracking().AnyAsync(x => x.DID.Equals(id));
        }
        #endregion
    }
}