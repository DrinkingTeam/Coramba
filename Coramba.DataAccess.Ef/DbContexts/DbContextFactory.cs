using System;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DataAccess.Ef.DbContexts
{
    public class DbContextFactory<TDbContext> : IDbContextFactory<TDbContext>
        where TDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Action<TDbContext> _configurator;
        public string Name { get; }

        public DbContextFactory(string name, Action<TDbContext> configurator, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _configurator = configurator;
        }

        public TDbContext Create()
        {
            var dbContext = ActivatorUtilities.CreateInstance<TDbContext>(_serviceProvider);

            _configurator?.Invoke(dbContext);

            return dbContext;
        }
    }
}