using Coramba.DependencyInjection.Annotations;
using Coramba.DependencyInjection.Modules;
using Coramba.Services.Common;
using Coramba.Services.Crud;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Coramba.Services
{
    [AutoModule(DependsOn = new[]
    {
        typeof(DataAccess.RegistrationModule)
    })]
    public class RegistrationModule : ModuleBuilder<RegistrationModule, RegistrationModule.ComponentInfo>
    {
        public class ComponentInfo
        {
        }

        protected override void RegisterOnce()
        {
            Services.TryAddScoped(typeof(ICrudModelService<>), typeof(CrudModelService<>));
            Services.TryAddScoped(typeof(BaseModelServiceContext<>));

            Services.TryAddScoped(typeof(ModelSelectorContext<,>));
            Services.TryAddScoped(typeof(ModelSelectorContext<,,>));
            Services.TryAddScoped(typeof(CudModelServiceContext<,,>));
            Services.TryAddScoped(typeof(CrudModelServiceContext<>));
        }
    }
}
