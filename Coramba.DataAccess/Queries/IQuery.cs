using System.Linq;
using System.Threading.Tasks;

namespace Coramba.DataAccess.Queries
{
    public interface IQuery<T, TFilter>
    {
        Task<IQueryable<T>> QueryAsync(IQueryable<T> queryable, TFilter filter);
    }
}
