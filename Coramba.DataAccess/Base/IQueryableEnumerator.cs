using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coramba.DataAccess.Base
{
    public interface IQueryableEnumerator<T>
    {
        Task<IEnumerable<T>> ToEnumerableAsync(IQueryable<T> queryable);
    }
}
