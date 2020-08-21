using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coramba.DataAccess.Common;
using Coramba.DataAccess.Repositories;

namespace Coramba.Services.Crud
{
    public abstract class ModelSelector<TModel, TModelDto, TId>: ISelectOneModelService<TModelDto>, ISelectAllModelService<TModelDto>, IModelSelector<TModel, TModelDto>
    {
        protected IRepository<TModel> Repository { get; }
        protected IQueryTransformer<TModel, TModelDto> QueryTransformer { get; }

        protected ModelSelector(ModelSelectorContext<TModel, TModelDto, TId> context)
        {
            Repository = context.Repository;
            QueryTransformer = context.QueryTransformer;
        }

        protected virtual Task<IQueryable<TModel>> GetQueryAsync()
            => Repository.QueryAsync();

        protected virtual async Task<IEnumerable<TModelDto>> TransformCoreAsync(IQueryable<TModel> queryable)
            => await ToDto(queryable);

        protected virtual async Task<IEnumerable<TModelDto>> TransformCoreAsync(IEnumerable<TModel> enumerable)
            => await ToDto(enumerable);

        protected abstract Task<IQueryable<TModel>> GetModelById(IQueryable<TModel> queryable, TId id);

        protected Task<IEnumerable<TModelDto>> ToDto(IQueryable<TModel> queryable)
            => QueryTransformer.TransformAsync(queryable);

        protected Task<IEnumerable<TModelDto>> ToDto(IEnumerable<TModel> enumerable)
            => QueryTransformer.TransformAsync(enumerable);

        protected async Task<IEnumerable<TModelDto>> ToDto(Task<IQueryable<TModel>> queryable)
            => await ToDto(await queryable);

        protected async Task<IEnumerable<TModelDto>> TransformCoreAsync(Task<IQueryable<TModel>> queryableTask)
            => await TransformCoreAsync(await queryableTask);

        protected async Task<TModelDto> SelectOneCoreAsync(IQueryable<TModel> queryable)
            => (await TransformCoreAsync(queryable)).FirstOrDefault();

        protected async Task<TModelDto> SelectOneCoreAsync(Task<IQueryable<TModel>> queryableTask)
            => await SelectOneCoreAsync(await queryableTask);

        public async Task<TModelDto> SelectOneAsync(object id)
            => await SelectOneCoreAsync(GetModelById(await GetQueryAsync(), (TId)id));

        public async Task<IEnumerable<TModelDto>> SelectAllAsync()
            => await TransformCoreAsync(await GetQueryAsync());

        public Task<IEnumerable<TModelDto>> SelectAsync(IQueryable<TModel> queryable)
            => TransformCoreAsync(queryable);

        public Task<IEnumerable<TModelDto>> SelectAsync(IEnumerable<TModel> queryable)
            => TransformCoreAsync(queryable);
    }
}
