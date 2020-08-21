using System;
using System.Collections.Generic;
using Coramba.DataAccess.Ef.DbContexts;
using Coramba.DataAccess.Ef.Repositories;
using Coramba.DataAccess.Ef.Transactions;
using Coramba.DependencyInjection.Annotations;
using Coramba.DependencyInjection.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Coramba.DataAccess.Ef
{
    [AutoModule(DependsOn = new[]
    {
        typeof(DataAccess.RegistrationModule)
    })]
    public class RegistrationModule : ModuleBuilder<RegistrationModule, RegistrationModule.ComponentInfo>
    {
        public class ComponentInfo
        {
            public List<(string Name, Type DbContextType, Action<DbContext> Configurator)> DbContextFactories
                = new List<(string Name, Type DbContextType, Action<DbContext> Configurator)>();
        }

        public RegistrationModule DbContextFactory<TDbContext>(string name, Action<TDbContext> configurator = null)
            where TDbContext: DbContext
        {
            configurator ??= db => { };
            return WithComponent(c =>
                c.DbContextFactories.Add((name, typeof(TDbContext), db => configurator((TDbContext) db))));
        }

        protected override void Register()
        {
            Component
                .DbContextFactories
                .ForEach(f =>
                {
                    Services.AddDbContextFactory(f.Name, f.DbContextType, f.Configurator);
                });
        }

        protected override void RegisterOnce()
        {
            Services.TryAddScoped(typeof(IEfTransactionFactory<>), typeof(EfTransactionFactory<>));
            Services.TryAddScoped(typeof(EfCrudRepositoryContext<,>));
            Services.TryAddScoped(typeof(IDbContextStore<>), typeof(DbContextStore<>));
            Services.TryAddScoped(typeof(IDbContextGetter<>), typeof(DbContextGetter<>));
        }
    }
}
