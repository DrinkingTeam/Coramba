using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coramba.Common;
using Microsoft.Extensions.Logging;

namespace Coramba.DataAccess.Ef.DbContexts
{
    class DbContextStore<TDbContext> : IDbContextStore<TDbContext>
        where TDbContext: Microsoft.EntityFrameworkCore.DbContext
    {
        private readonly ILogger<DbContextStore<TDbContext>> _logger;
        private readonly IDbContextFactory<TDbContext>[] _dbContextFactories;
        private string _currentName;

        private Dictionary<string, TDbContext> _dbContexts;

        private Dictionary<string, TDbContext> DbContexts => _dbContexts ??= CreateDbContexts();

        public DbContextStore(IEnumerable<IDbContextFactory<TDbContext>> dbContextFactories,
            ILogger<DbContextStore<TDbContext>> logger)
        {
            _logger = logger;
            _dbContextFactories = dbContextFactories?.ToArray() ?? new IDbContextFactory<TDbContext>[0];
            _currentName = _dbContextFactories.FirstOrDefault()?.Name;
        }

        private Dictionary<string, TDbContext> CreateDbContexts()
        {
            var result = new Dictionary<string, TDbContext>();
            _dbContextFactories
                .ForEach(f =>
                {
                    result.Add(f.Name, f.Create());
                });
            return result;
        }

        public TDbContext Get(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (!DbContexts.TryGetValue(name, out var dbContext))
                throw new Exception($"DbContext with name {name} is not registered");
            return dbContext;
        }

        public string GetCurrent() => _currentName;

        public void SetCurrent(string name)
        {
            _logger.LogInformation($"Set db context for {typeof(TDbContext).Name}: {name}");

            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!DbContexts.ContainsKey(name))
                throw new Exception($"DbContext with name {name} is not registered");

            _currentName = name;
        }

        public void Add(string name, TDbContext dbContext)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

            DbContexts.Add(name, dbContext);
        }

        public void Remove(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            if (!DbContexts.ContainsKey(name))
                throw new Exception($"DbContext with name {name} is not registered");

            DbContexts.Remove(name);
            if (_currentName == name)
                _currentName = null;
        }

        private void ReleaseUnmanagedResources()
        {
            _dbContexts?.ForEach(x => x.Value?.Dispose());
            _dbContexts = null;
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~DbContextStore()
        {
            ReleaseUnmanagedResources();
        }

        public async ValueTask DisposeAsync()
        {
            if (_dbContexts != null)
            {
                await _dbContexts.ForEachAsync(async x =>
                {
                    if (x.Value != null)
                        await x.Value.DisposeAsync();
                });
                _dbContexts = null;
            }
        }
    }
}