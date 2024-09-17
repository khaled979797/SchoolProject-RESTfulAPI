using Microsoft.EntityFrameworkCore;
using SchoolProject.Data.Entities;
using SchoolProject.Infrastructure.Abstracts;
using SchoolProject.Infrastructure.Data;
using SchoolProject.Infrastructure.InfrastructureBases;

namespace SchoolProject.Infrastructure.Repositories
{
    public class SubjectRepository : GenericRepositoryAsync<Subjects>, ISubjectRepository
    {
        #region Fields
        private readonly DbSet<Subjects> subjects;
        #endregion

        #region Constructor
        public SubjectRepository(AppDbContext context) : base(context)
        {
            subjects = context.Set<Subjects>();
        }
        #endregion

        #region Functions
        #endregion
    }
}
