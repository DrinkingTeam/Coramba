using System;
using System.Linq;
using System.Threading.Tasks;
using Coramba.DataAccess.Ef.DbContexts;
using Coramba.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Coramba.DataAccess.Ef.Repositories
{
    public class EfCrudRepository<TDbContext, T> : CrudRepository<T>
        where TDbContext : DbContext
        where T : class
    {
        protected IDbContextGetter<TDbContext> DbContextGetter { get; }

        protected TDbContext DbContext
        {
            get
            {
                var dbContext = DbContextGetter.Get();
                if (dbContext == null)
                    throw new Exception($"DbContext {typeof(TDbContext)} not set");
                return dbContext;
            }
        }

        protected DbSet<T> DbSet => DbContext.Set<T>();

        public EfCrudRepository(EfCrudRepositoryContext<TDbContext, T> context)
            : base(context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            DbContextGetter = context.DbContextGetter;
        }

        protected override RepositoryOperationContext CreateOperationContext()
        {
            return new EfRepositoryOperationContext(DbContext);
        }

        protected override Task<IQueryable<T>> ListCoreAsync(RepositoryOperationContext context)
            => Task.FromResult((IQueryable<T>)DbSet);

        protected override async Task InsertCoreAsync(T entity, RepositoryOperationContext context)
        {
            DbSet.Add(entity);
            await DbContext.SaveChangesAsync();
        }

        protected override async Task UpdateCoreAsync(T entity, RepositoryOperationContext context)
        {
            DbSet.Update(entity);
            await DbContext.SaveChangesAsync();
        }

        protected override async Task DeleteCoreAsync(T entity, RepositoryOperationContext context)
        {
            DbSet.Remove(entity);
            await DbContext.SaveChangesAsync();
        }
    }
}
