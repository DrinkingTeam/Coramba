namespace Coramba.DataAccess.Ef.DbContexts
{
    public interface IDbContextStore<TDbContext>
    {
        TDbContext Get(string name);
        string GetCurrent();
        void SetCurrent(string name);
        void Add(string name, TDbContext dbContext);
        void Remove(string name);
    }
}
