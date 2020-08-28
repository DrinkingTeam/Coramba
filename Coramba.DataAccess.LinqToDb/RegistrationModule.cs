using System;
using System.Collections.Generic;
using Coramba.DataAccess.LinqToDb.DataConnections;
using Coramba.DataAccess.LinqToDb.Repositories;
using Coramba.DataAccess.LinqToDb.Transactions;
using Coramba.DependencyInjection.Annotations;
using Coramba.DependencyInjection.Modules;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Coramba.DataAccess.LinqToDb
{
    [AutoModule(DependsOn = new[]
    {
        typeof(DataAccess.RegistrationModule)
    })]
    public class RegistrationModule : ModuleBuilder<RegistrationModule, RegistrationModule.ComponentInfo>
    {
        public class ComponentInfo
        {
            public List<(string Name, Type DbContextType, Action<DataConnection> Configurator)> DbContextFactories
                = new List<(string Name, Type DbContextType, Action<DataConnection> Configurator)>();
        }

        public RegistrationModule DbContextFactory<TDataConnection>(string name, Action<TDataConnection> configurator = null)
            where TDataConnection: DataConnection
        {
            configurator ??= db => { };
            return WithComponent(c =>
                c.DbContextFactories.Add((name, typeof(TDataConnection), db => configurator((TDataConnection) db))));
        }

        protected override void Register()
        {
            Component
                .DbContextFactories
                .ForEach(f =>
                {
                    Services.AddDataConnectionFactory(f.Name, f.DbContextType, f.Configurator);
                });
        }

        protected override void RegisterOnce()
        {
            Services.TryAddScoped(typeof(ILinqToDbTransactionFactory<>), typeof(LinqToDbTransactionFactory<>));
            Services.TryAddScoped(typeof(LinqToDbCrudRepositoryContext<,>));
            Services.TryAddScoped(typeof(IDataConnectionStore<>), typeof(DataConnectionStore<>));
            Services.TryAddScoped(typeof(IDataConnectionGetter<>), typeof(DataConnectionGetter<>));
        }
    }
}
