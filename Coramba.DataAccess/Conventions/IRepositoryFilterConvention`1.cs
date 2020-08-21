using System.Linq;
using System.Threading.Tasks;
using Coramba.DataAccess.Repositories;

namespace Coramba.DataAccess.Conventions
{
    public interface IRepositoryFilterConvention<T>: IRepositoryConvention<T>
    {
        Task<IQueryable<T>> ApplyAsync(IQueryable<T> query, RepositoryOperationContext context);
    }
}
