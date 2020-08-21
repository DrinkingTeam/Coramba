namespace Coramba.DataAccess.Ef.DbContexts
{
    public interface IDbContextFactory<TDbContext>
        where TDbContext: Microsoft.EntityFrameworkCore.DbContext
    {
        string Name { get; }
        TDbContext Create();
    }
}
