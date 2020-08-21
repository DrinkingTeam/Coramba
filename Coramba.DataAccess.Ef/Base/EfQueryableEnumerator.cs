using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coramba.DataAccess.Base;
using Microsoft.EntityFrameworkCore;

namespace Coramba.DataAccess.Ef.Base
{
    public class EfQueryableEnumerator<T>: IQueryableEnumerator<T>
    {
        public async Task<IEnumerable<T>> ToEnumerableAsync(IQueryable<T> queryable)
        {
            if (queryable is IAsyncEnumerable<T>)
                return await queryable.ToListAsync();
            return queryable.ToList();
        }
    }
}
