using System;

namespace Coramba.DataAccess.Ef.DbContexts
{
    public static class DbContextFactoryExtensions
    {
        public static TDbContext Create<TDbContext>(this IDbContextFactory<TDbContext> factory, Action<TDbContext> configurator)
            where TDbContext : Microsoft.EntityFrameworkCore.DbContext
        {
            var result = factory.Create();
            configurator?.Invoke(result);
            return result;
        }
    }
}
