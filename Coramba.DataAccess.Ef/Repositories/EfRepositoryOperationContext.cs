using Coramba.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Coramba.DataAccess.Ef.Repositories
{
    public class EfRepositoryOperationContext: RepositoryOperationContext
    {
        public EfRepositoryOperationContext(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public DbContext DbContext { get; }
    }
}
