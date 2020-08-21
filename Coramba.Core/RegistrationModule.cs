using Coramba.Core.AutoMapper;
using Coramba.Core.Converters;
using Coramba.Core.Parallel;
using Coramba.DependencyInjection.Annotations;
using Coramba.DependencyInjection.Modules;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Coramba.Core
{
    [AutoModule(DependsOn = new[]
    {
        typeof(DependencyInjection.RegistrationModule)
    })]
    public class RegistrationModule: ModuleBuilder<RegistrationModule, object>
    {
        protected override void RegisterOnce()
        {
            Services.TryAddScoped<IParallelRunner, ParallelRunner>();
            Services.TryAddScoped(typeof(IObjectConverter<,>), typeof(AutoMapperObjectConverter<,>));
        }
    }
}
