namespace Coramba.DataAccess.Ef.DbContexts
{
    public interface IDbContextGetter<TDbContext>
        where TDbContext: Microsoft.EntityFrameworkCore.DbContext
    {
        TDbContext Get();
    }
}
