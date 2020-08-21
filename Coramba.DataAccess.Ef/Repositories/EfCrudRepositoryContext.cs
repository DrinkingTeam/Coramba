using System;
using Coramba.DataAccess.Conventions;
using Coramba.DataAccess.Ef.DbContexts;
using Coramba.DataAccess.Repositories;
using Microsoft.Extensions.Logging;

namespace Coramba.DataAccess.Ef.Repositories
{
    public class EfCrudRepositoryContext<TDbContext, T> : CrudRepositoryContext<T>
        where TDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public IDbContextGetter<TDbContext> DbContextGetter { get; }

        public EfCrudRepositoryContext(IRepositoryConventions<T> conventions, IDbContextGetter<TDbContext> dbContextGetter, ILogger<T> logger)
            : base(conventions, logger)
        {
            DbContextGetter = dbContextGetter ?? throw new ArgumentNullException(nameof(dbContextGetter));
        }
    }
}
