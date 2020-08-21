using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coramba.DataAccess.Common
{
    public interface IQueryTransformer<TModelFrom, TModelTo>
    {
        Task<IEnumerable<TModelTo>> TransformAsync(IQueryable<TModelFrom> query);
        Task<IEnumerable<TModelTo>> TransformAsync(IEnumerable<TModelFrom> query);
    }
}
