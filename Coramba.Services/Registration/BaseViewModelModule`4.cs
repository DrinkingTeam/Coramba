using Coramba.DataAccess.Queries.Universal;
using Coramba.Services.Crud;
using Coramba.Services.Filter;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.Services.Registration
{
    public abstract class BaseModelDtoModule<TModelDto, TId, TModel, TModule> : BaseModelDtoModule<TModelDto, TId, TModule>
        where TModule : BaseModelDtoModule<TModelDto, TId, TModel, TModule>
    {
        public TModule ModelSelector<TService>()
            where TService : ISelectAllModelService<TModelDto>, ISelectOneModelService<TModelDto>
            => SelectAllModelService<TService>()
                .SelectOneModelService<TService>()
                .WithComponent(c => c.ModelSelectorServiceType = typeof(TService));

        public TModule SelectModelServiceFilter<TFilter, TFilterDto>()
            => SelectModelService<SelectModelService<TModel, TModelDto, TFilter, TFilterDto>, TFilterDto>();

        public TModule SelectModelServiceFilter<TFilterDto>()
            => SelectModelServiceFilter<UniversalFilter, TFilterDto>();

        protected override void Register()
        {
            Component.InsertModelServiceType ??= typeof(CudModelService<TModel, TModelDto, TId>);
            Component.UpdateModelServiceType ??= typeof(CudModelService<TModel, TModelDto, TId>);
            Component.DeleteModelServiceType ??= typeof(CudModelService<TModel, TModelDto, TId>);
            Component.SelectOneModelServiceType ??= typeof(ModelSelector<TModel, TModelDto>);
            Component.SelectAllModelServiceType ??= typeof(ModelSelector<TModel, TModelDto>);
            Component.ModelSelectorServiceType ??= typeof(ModelSelector<TModel, TModelDto>);
            Component.SelectModelServiceTypes.TryAdd(typeof(FilterDto<TId>), typeof(SelectModelService<TModel, TModelDto, UniversalFilter, FilterDto<TId>>));

            base.Register();

            if (Component.ModelSelectorServiceType != null)
                Services.AddScoped(typeof(IModelSelector<TModel, TModelDto>), Component.ModelSelectorServiceType);
        }
    }
}
