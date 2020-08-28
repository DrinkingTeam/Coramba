using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coramba.DataAccess.Base;
using LinqToDB;

namespace Coramba.DataAccess.LinqToDb.Base
{
    public class LinqToDbQueryableEnumerator<T>: IQueryableEnumerator<T>
    {
        public async Task<IEnumerable<T>> ToEnumerableAsync(IQueryable<T> queryable)
        {
            if (queryable is IAsyncEnumerable<T>)
                return await queryable.ToListAsync();
            return queryable.ToList();
        }
    }
}
