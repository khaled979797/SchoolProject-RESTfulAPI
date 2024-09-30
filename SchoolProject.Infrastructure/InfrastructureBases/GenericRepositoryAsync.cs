using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SchoolProject.Infrastructure.Data;

namespace SchoolProject.Infrastructure.InfrastructureBases
{
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : class
    {
        #region Vars / Props

        protected readonly AppDbContext context;

        #endregion

        #region Constructor(s)
        public GenericRepositoryAsync(AppDbContext context)
        {
            this.context = context;
        }

        #endregion


        #region Methods

        #endregion

        #region Actions
        public virtual async Task<T> GetByIdAsync(int id)
        {

            return await context.Set<T>().FindAsync(id);
        }


        public IQueryable<T> GetTableNoTracking()
        {
            return context.Set<T>().AsNoTracking().AsQueryable();
        }


        public virtual async Task AddRangeAsync(ICollection<T> entities)
        {
            await context.Set<T>().AddRangeAsync(entities);
            await context.SaveChangesAsync();

        }
        public virtual async Task<T> AddAsync(T entity)
        {
            await context.Set<T>().AddAsync(entity);
            await context.SaveChangesAsync();

            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            context.Set<T>().Update(entity);
            await context.SaveChangesAsync();

        }

        public virtual async Task DeleteAsync(T entity)
        {
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
        }
        public virtual async Task DeleteRangeAsync(ICollection<T> entities)
        {
            foreach (var entity in entities)
            {
                context.Entry(entity).State = EntityState.Deleted;
            }
            await context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }



        public IDbContextTransaction BeginTransaction()
        {


            return context.Database.BeginTransaction();
        }

        public void Commit()
        {
            context.Database.CommitTransaction();

        }

        public void RollBack()
        {
            context.Database.RollbackTransaction();

        }

        public IQueryable<T> GetTableAsTracking()
        {
            return context.Set<T>().AsQueryable();

        }

        public virtual async Task UpdateRangeAsync(ICollection<T> entities)
        {
            context.Set<T>().UpdateRange(entities);
            await context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            await context.Database.CommitTransactionAsync();
        }

        public async Task RollBackAsync()
        {
            await context.Database.RollbackTransactionAsync();
        }
        #endregion
    }
}
