using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coramba.Services.Crud
{
    public class CrudModelService<TModelDto, TId> : ICrudModelService<TModelDto>
    {
        protected IInsertModelService<TModelDto> InsertModelService { get; }
        protected IUpdateModelService<TModelDto> UpdateModelService { get; }
        protected IDeleteModelService<TModelDto> DeleteModelService { get; }
        protected ISelectOneModelService<TModelDto> SelectOneModelService { get; }
        protected ISelectAllModelService<TModelDto> SelectAllModelService { get; }

        public CrudModelService(CrudModelServiceContext<TModelDto> context)
        {
            InsertModelService = context.InsertModelService;
            UpdateModelService = context.UpdateModelService;
            DeleteModelService = context.DeleteModelService;
            SelectOneModelService = context.SelectOneModelService;
            SelectAllModelService = context.SelectAllModelService;
        }

        protected virtual Task<TModelDto> GetCoreAsync(TId id)
            => SelectOneModelService.SelectOneAsync(id);

        public Task<TModelDto> GetAsync(TId id)
            => GetCoreAsync(id);

        protected virtual Task<IEnumerable<TModelDto>> SelectAllCoreAsync()
            => SelectAllModelService.SelectAllAsync();

        public Task<IEnumerable<TModelDto>> SelectAllAsync()
            => SelectAllCoreAsync();

        protected virtual Task<TModelDto> InsertCoreAsync(TModelDto modelDto)
            => InsertModelService.InsertAsync(modelDto);

        public Task<TModelDto> InsertAsync(TModelDto modelDto)
            => InsertCoreAsync(modelDto);

        protected virtual Task<TModelDto> UpdateCoreAsync(TModelDto modelDto)
            => UpdateModelService.UpdateAsync(modelDto);

        public Task<TModelDto> UpdateAsync(TModelDto modelDto)
            => UpdateCoreAsync(modelDto);

        protected virtual Task<bool> DeleteCoreAsync(TId id)
            => DeleteModelService.DeleteAsync(id);

        public Task<bool> DeleteAsync(TId id)
            => DeleteCoreAsync(id);

        Task<TModelDto> ISelectOneModelService<TModelDto>.SelectOneAsync(object id)
            => GetAsync((TId)id);

        Task<bool> IDeleteModelService<TModelDto>.DeleteAsync(object id)
            => DeleteAsync((TId)id);
    }
}