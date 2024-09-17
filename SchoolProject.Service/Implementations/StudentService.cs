using Microsoft.EntityFrameworkCore;
using SchoolProject.Data.Entities;
using SchoolProject.Data.Enums;
using SchoolProject.Infrastructure.Abstracts;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Service.Implementations
{
    public class StudentService(IStudentRepository studentRepository) : IStudentService
    {
        #region Handle Fuctions
        public async Task<List<Student>> GetStudentsListAsync()
        {
            return await studentRepository.GetStudentsListAsync();
        }
        public async Task<Student> GetStudentByIdWithIncludeAsync(int id)
        {
            var student = studentRepository.GetTableNoTracking().Include(x => x.Department).Where(x => x.StudID == id).FirstOrDefault();
            return student;
        }

        public async Task<string> AddAsync(Student student)
        {
            // Add Student
            await studentRepository.AddAsync(student);
            return "Success";
        }

        public async Task<bool> IsNameArExist(string nameAr)
        {
            var studentInDb = studentRepository.GetTableNoTracking().Where(x => x.NameAr.Equals(nameAr)).FirstOrDefault();
            if (studentInDb == null) return false;
            return true;
        }
        public async Task<bool> IsNameEnExist(string nameEn)
        {
            var studentInDb = studentRepository.GetTableNoTracking().Where(x => x.NameEn.Equals(nameEn)).FirstOrDefault();
            if (studentInDb == null) return false;
            return true;
        }

        public async Task<bool> IsNameArExistExcludeSelf(string nameAr, int id)
        {
            var studentInDb = await studentRepository.GetTableNoTracking().Where(x => x.NameAr.Equals(nameAr) & !x.StudID.Equals(id)).FirstOrDefaultAsync();
            if (studentInDb == null) return false;
            return true;
        }

        public async Task<bool> IsNameEnExistExcludeSelf(string nameEn, int id)
        {
            var studentInDb = await studentRepository.GetTableNoTracking().Where(x => x.NameEn.Equals(nameEn) & !x.StudID.Equals(id)).FirstOrDefaultAsync();
            if (studentInDb == null) return false;
            return true;
        }

        public async Task<string> EditAsync(Student student)
        {
            // Edit Student
            await studentRepository.UpdateAsync(student);
            return "Success";
        }

        public async Task<string> DeleteAsync(Student student)
        {
            var trans = studentRepository.BeginTransaction();
            try
            {
                await studentRepository.DeleteAsync(student);
                await trans.CommitAsync();
                return "Success";
            }
            catch
            {
                await trans.RollbackAsync();
                return "Failed";
            }
        }

        public async Task<Student> GetByIdAsync(int id)
        {
            var student = await studentRepository.GetByIdAsync(id);
            return student;
        }

        public IQueryable<Student> GetStudentsQuerable()
        {
            return studentRepository.GetTableNoTracking().Include(x => x.Department).AsQueryable();
        }

        public IQueryable<Student> FilterStudentPaginatedQuerable(StudentOrderingEnum orderingEnum, string search)
        {
            var querable = studentRepository.GetTableNoTracking().Include(x => x.Department).AsQueryable();
            if (search != null)
            {
                querable = querable.Where(x => x.NameEn.Contains(search) || x.Address.Contains(search));
            }

            switch (orderingEnum)
            {
                case StudentOrderingEnum.StudID:
                    querable = querable.OrderBy(x => x.StudID);
                    break;

                case StudentOrderingEnum.Name:
                    querable = querable.OrderBy(x => x.NameEn);
                    break;

                case StudentOrderingEnum.Address:
                    querable = querable.OrderBy(x => x.Address);
                    break;

                case StudentOrderingEnum.DepartmentName:
                    querable = querable.OrderBy(x => x.Department.DNameEn);
                    break;
            }
            return querable;
        }

        public IQueryable<Student> GetStudentsByDepartmentIdQuerable(int id)
        {
            return studentRepository.GetTableNoTracking().Where(x => x.DID.Equals(id)).AsQueryable();

        }
        #endregion
    }
}
