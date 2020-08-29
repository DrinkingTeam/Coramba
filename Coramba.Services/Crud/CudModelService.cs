using System.Linq;
using System.Threading.Tasks;
using Coramba.Core.Converters;
using Coramba.DataAccess.Base;
using Coramba.DataAccess.Repositories;

namespace Coramba.Services.Crud
{
    public class CudModelService<TModel, TModelDto, TId> :
        IInsertModelService<TModelDto>,
        IUpdateModelService<TModelDto>,
        IDeleteModelService<TModelDto>
    {
        protected ICrudRepository<TModel> Repository { get; }
        protected IModelFinder<TModel> ModelFinder { get; }
        protected IObjectConverter<TModelDto, TId> DtoToIdConverter { get; }
        protected IObjectConverter<TModelDto, TModel> DtoToModelConverter { get; }
        protected IObjectConverter<TModel, TModelDto> ModelToDtoConverter { get; }
        protected IQueryableEnumerator<TModel> QueryableEnumerator { get; }
        public ISelectOneModelService<TModelDto> SelectOneModelService { get; }

        public CudModelService(CudModelServiceContext<TModel, TModelDto, TId> context)
        {
            Repository = context.Repository;
            ModelFinder = context.ModelFinder;
            DtoToIdConverter = context.DtoToIdConverter;
            ModelToDtoConverter = context.ModelToDtoConverter;
            DtoToModelConverter = context.DtoToModelConverter;
            QueryableEnumerator = context.QueryableEnumerator;
            SelectOneModelService = context.SelectOneModelService;
        }

        protected virtual Task<bool> ValidateDtoAsync(TModelDto modelDto, bool insert)
        {
            return Task.FromResult(true);
        }

        protected virtual Task InsertCoreAsync(TModel model, TModelDto modelDto)
            => Repository.InsertAsync(model);

        protected virtual Task<TModel> GetInsertModelAsync(TModelDto modelDto, TModel model)
            => DtoToModelConverter.ConvertAsync(modelDto, model);

        public async Task<TModelDto> InsertAsync(TModelDto modelDto)
        {
            if (!await ValidateDtoAsync(modelDto, true))
                return default;

            var model = await GetInsertModelAsync(modelDto, await Repository.NewAsync());

            await InsertCoreAsync(model, modelDto);

            modelDto = await ModelToDtoConverter.Convert(model).ToAsync();

            var id = await DtoToIdConverter.Convert(modelDto).ToAsync();

            return await SelectOneModelService.SelectOneAsync(id);
        }

        protected virtual Task UpdateCoreAsync(TModel model, TModelDto modelDto)
            => Repository.UpdateAsync(model);

        protected virtual async Task<TModel> GetModelById(IQueryable<TModel> queryable, TId id)
        {
            queryable = await ModelFinder.FindAsync(queryable, id);
            return (await queryable.EnumerateAsync(QueryableEnumerator)).FirstOrDefault();
        }

        protected virtual async Task<TModel> GetUpdateModelAsync(TModelDto modelDto, TModel model)
            => await DtoToModelConverter.Convert(modelDto).ToAsync(model);

        public async Task<TModelDto> UpdateAsync(TModelDto modelDto)
        {
            if (!await ValidateDtoAsync(modelDto, false))
                return default;

            var id = await DtoToIdConverter.Convert(modelDto).ToAsync();

            var model = await GetModelById(await Repository.QueryAsync(), id);
            if (model == null)
                return default;

            model = await GetUpdateModelAsync(modelDto, model);

            await UpdateCoreAsync(model, modelDto);

            return await SelectOneModelService.SelectOneAsync(id);
        }

        protected virtual Task DeleteCoreAsync(TModel model)
            => Repository.DeleteAsync(model);

        public async Task<bool> DeleteAsync(object id)
        {
            var model = await GetModelById(await Repository.QueryAsync(), (TId)id);
            if (model == null)
                return false;

            await DeleteCoreAsync(model);

            return true;
        }
    }
}
