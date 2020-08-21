using System;
using Autodesk.Forge.Client;
using Coramba.DataAccess.Ef;
using Coramba.DataAccess.Forge.Api;
using Coramba.DataAccess.Forge.Auth;
using Coramba.DataAccess.Forge.PropertiesDb;
using Coramba.DataAccess.Forge.Services;
using Coramba.DependencyInjection.Annotations;
using Coramba.DependencyInjection.Modules;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Coramba.DataAccess.Forge
{
    [AutoModule(DependsOn = new[]
    {
        typeof(Ef.RegistrationModule)
    })]
    public class RegistrationModule: ModuleBuilder<RegistrationModule, RegistrationModule.ComponentInfo>
    {
        public class ComponentInfo
        {
            public Func<IServiceProvider, Configuration> Configurator { get; set; }
        }

        public RegistrationModule Configurator(Func<IServiceProvider, Configuration> configurator)
            => WithComponent(c => c.Configurator = configurator);

        protected override void RegisterOnce()
        {
            var configurator = Component.Configurator ?? (sp =>
            {
                var cfg = new Configuration
                {
                    Timeout = 20 * 60 * 1000
                };

                return cfg;
            });
            Services.TryAddScoped(configurator);

            Services.TryAddScoped<IForgeAuthService, ForgeAuthService>();
            Services.TryAddScoped<IForgeAuth, ForgeAuth>();
            Services.TryAddScoped(typeof(IForgeApiRunner<>), typeof(ForgeApiRunner<>));
            Services.TryAddSingleton<IForgeAuthCache, ForgeAuthCache>();
            Services.TryAddScoped<IForgeRawApiRunner, ForgeRawApiRunner>();

            Services.TryAddScoped<IForgeObjectsApiService, ForgeObjectsApiService>();
            Services.TryAddScoped<IForgeDerivativesApiService, ForgeDerivativesApiService>();
            Services.TryAddScoped<IForgeBucketsApiService, ForgeBucketsApiService>();

            Services.TryAddScoped<IForgePropertiesDbFactory, ForgePropertiesDbFactory>();

            Services.AddDbContextFactory<ForgePropertiesDbContext>("ForgeProperties");
            Services.AddEntities(
                typeof(ForgePropertyObject), typeof(ForgePropertyObjectAttribute),
                typeof(ForgePropertyObjectValue), typeof(ForgePropertyObjectEav));
        }
    }
}
