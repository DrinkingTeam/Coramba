using System;

namespace Coramba.DataAccess.Ef.DbContexts
{
    public interface IDbContextStore<TDbContext>: IDisposable, IAsyncDisposable
    {
        TDbContext Get(string name);
        string GetCurrent();
        void SetCurrent(string name);
        void Add(string name, TDbContext dbContext);
        void Remove(string name);
    }
}
