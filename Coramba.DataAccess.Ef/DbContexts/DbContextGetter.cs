using System;

namespace Coramba.DataAccess.Ef.DbContexts
{
    public class DbContextGetter<TDbContext> : IDbContextGetter<TDbContext>
        where TDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private readonly IDbContextStore<TDbContext> _store;

        public DbContextGetter(IDbContextStore<TDbContext> store)
        {
            _store = store;
        }

        public TDbContext Get()
        {
            var name = _store.GetCurrent();
            if (name == null)
                throw new Exception($"Current DbContext is not set for {typeof(TDbContext).Name}");
            return _store.Get(name);
        }
    }
}