using Microsoft.EntityFrameworkCore;
using SchoolProject.Data.Entities.Identity;
using SchoolProject.Infrastructure.Abstracts;
using SchoolProject.Infrastructure.Data;
using SchoolProject.Infrastructure.InfrastructureBases;

namespace SchoolProject.Infrastructure.Repositories
{
    public class RefreshTokenRepository : GenericRepositoryAsync<UserRefreshToken>, IRefreshTokenRepository
    {
        #region Fields
        private readonly DbSet<UserRefreshToken> userRefreshToken;
        #endregion

        #region Constructor
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
            userRefreshToken = context.Set<UserRefreshToken>();
        }
        #endregion

        #region Functions
        #endregion
    }
}
