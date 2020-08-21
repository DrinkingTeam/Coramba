using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coramba.DataAccess.Base
{
    public static class QueryableExtensions
    {
        public static async Task<IEnumerable<T>> EnumerateAsync<T>(this IQueryable<T> list, IQueryableEnumerator<T> queryableEnumerator)
            => await queryableEnumerator.ToEnumerableAsync(list);

        public static async Task<IEnumerable<T>> EnumerateAsync<T>(this Task<IQueryable<T>> listTask, IQueryableEnumerator<T> queryableEnumerator)
            => await queryableEnumerator.ToEnumerableAsync(await listTask);
    }
}
