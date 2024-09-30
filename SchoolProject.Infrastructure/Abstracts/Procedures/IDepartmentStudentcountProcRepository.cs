using SchoolProject.Data.Entities.Procedures;

namespace SchoolProject.Infrastructure.Abstracts.Procedures
{
    public interface IDepartmentStudentcountProcRepository
    {
        public Task<IReadOnlyCollection<DepartmentStudentcountProc>> GetDepartmentStudentcountProc(DepartmentStudentcountProcParameters parameters);
    }
}