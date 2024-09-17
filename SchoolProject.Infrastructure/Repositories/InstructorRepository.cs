using Microsoft.EntityFrameworkCore;
using SchoolProject.Data.Entities;
using SchoolProject.Infrastructure.Abstracts;
using SchoolProject.Infrastructure.Data;
using SchoolProject.Infrastructure.InfrastructureBases;

namespace SchoolProject.Infrastructure.Repositories
{
    public class InstructorRepository : GenericRepositoryAsync<Instructor>, IInstructorRepository
    {
        #region Fields
        private readonly DbSet<Instructor> instructors;
        #endregion

        #region Constructor
        public InstructorRepository(AppDbContext context) : base(context)
        {
            instructors = context.Set<Instructor>();
        }
        #endregion

        #region Functions
        #endregion
    }
}
