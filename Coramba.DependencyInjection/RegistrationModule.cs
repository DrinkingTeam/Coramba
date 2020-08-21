using Coramba.DependencyInjection.Annotations;
using Coramba.DependencyInjection.Modules;
using Coramba.DependencyInjection.ScopedServices;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Coramba.DependencyInjection
{
    [AutoModule]
    public sealed class RegistrationModule: ModuleBuilder<RegistrationModule, object>
    {
        protected override void RegisterOnce()
        {
            Services.TryAddScoped<IScopedRunner, ScopedRunner>();
            Services.TryAddScoped<IAsyncServiceScopeFactory, AsyncServiceScopeFactory>();
        }
    }
}
