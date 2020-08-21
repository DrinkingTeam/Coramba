using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coramba.Services.Crud
{
    public interface IModelSelector<TModel, TModelDto>
    {
        Task<IEnumerable<TModelDto>> SelectAsync(IQueryable<TModel> queryable);
        Task<IEnumerable<TModelDto>> SelectAsync(IEnumerable<TModel> queryable);
    }
}
