using Coramba.DataAccess.Conventions;
using Microsoft.Extensions.Logging;

namespace Coramba.DataAccess.Repositories
{
    public abstract class CrudRepositoryContext<T>: RepositoryContext<T>
    {
        protected CrudRepositoryContext(IRepositoryConventions<T> conventions, ILogger<T> logger)
            : base(conventions, logger)
        {
        }
    }
}
