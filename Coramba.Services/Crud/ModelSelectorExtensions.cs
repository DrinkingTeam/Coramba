using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coramba.Services.Crud
{
    public static class ModelSelectorExtensions
    {
        public static Task<IEnumerable<TModelDto>> SelectAsync<TModel, TModelDto>(this IQueryable<TModel> queryable, IModelSelector<TModel, TModelDto> selector)
            => selector.SelectAsync(queryable);

        public static async Task<IEnumerable<TModelDto>> SelectAsync<TModel, TModelDto>(this Task<IQueryable<TModel>> queryableTask, IModelSelector<TModel, TModelDto> selector)
            => await selector.SelectAsync(await queryableTask);
    }
}
