using Coramba.DependencyInjection;
using Coramba.DependencyInjection.Modules;
using Coramba.Services.Crud;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.Services.Registration
{
    public abstract class BaseModelDtoModule<TModelDto, TModule> : ModuleBuilder<TModule, ModelDtoModuleComponent>
        where TModule : BaseModelDtoModule<TModelDto, TModule>
    {
        internal TModule SetupComponent(ModelDtoModuleComponent component)
        {
            Component = component;
            return (TModule)this;
        }

        public TModule CudModelService<TService>()
            where TService : IInsertModelService<TModelDto>, IUpdateModelService<TModelDto>, IDeleteModelService<TModelDto>
            => InsertModelService<TService>()
                .UpdateModelService<TService>()
                .DeleteModelService<TService>();

        public TModule InsertModelService<TService>()
            where TService : IInsertModelService<TModelDto>
            => WithComponent(c => c.InsertModelServiceType = typeof(TService));

        public TModule UpdateModelService<TService>()
            where TService : IUpdateModelService<TModelDto>
            => WithComponent(c => c.UpdateModelServiceType = typeof(TService));

        public TModule DeleteModelService<TService>()
            where TService : IDeleteModelService<TModelDto>
            => WithComponent(c => c.DeleteModelServiceType = typeof(TService));

        public TModule SelectAllModelService<TService>()
            where TService : ISelectAllModelService<TModelDto>
            => WithComponent(c => c.SelectAllModelServiceType = typeof(TService));

        public TModule SelectOneModelService<TService>()
            where TService : ISelectOneModelService<TModelDto>
            => WithComponent(c => c.SelectOneModelServiceType = typeof(TService));

        protected override void Register()
        {
            Services.AddFromAnnotations(typeof(TModelDto));

            if (Component.InsertModelServiceType != null)
                Services.AddScoped(typeof(IInsertModelService<TModelDto>), Component.InsertModelServiceType);

            if (Component.UpdateModelServiceType != null)
                Services.AddScoped(typeof(IUpdateModelService<TModelDto>), Component.UpdateModelServiceType);

            if (Component.DeleteModelServiceType != null)
                Services.AddScoped(typeof(IDeleteModelService<TModelDto>), Component.DeleteModelServiceType);
    
            if (Component.SelectAllModelServiceType != null)
                Services.AddScoped(typeof(ISelectAllModelService<TModelDto>), Component.SelectAllModelServiceType);

            if (Component.SelectOneModelServiceType != null)
                Services.AddScoped(typeof(ISelectOneModelService<TModelDto>), Component.SelectOneModelServiceType);
        }
    }
}
