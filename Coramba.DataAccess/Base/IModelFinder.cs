using System.Linq;
using System.Threading.Tasks;

namespace Coramba.DataAccess.Base
{
    public interface IModelFinder<TModel>
    {
        Task<IQueryable<TModel>> FindAsync(IQueryable<TModel> queryable, object criteria);
    }
}
