using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coramba.DataAccess.Common
{
    public static class QueryTransformerExtensions
    {
        public static Task<IEnumerable<TModelTo>> TransformAsync<TModelFrom, TModelTo>(
            this IQueryable<TModelFrom> query,
            IQueryTransformer<TModelFrom, TModelTo> queryTransformer)
        {
            return queryTransformer.TransformAsync(query);
        }
    }
}
