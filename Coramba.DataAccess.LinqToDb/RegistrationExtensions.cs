using System;
using Coramba.DataAccess.Base;
using Coramba.DataAccess.Conventions;
using Coramba.DataAccess.LinqToDb.Base;
using Coramba.DataAccess.LinqToDb.DataConnections;
using Coramba.DataAccess.LinqToDb.Repositories;
using Coramba.DependencyInjection;
using LinqToDB.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Coramba.DataAccess.LinqToDb
{
    public static class RegistrationExtensions
    {
        public static RegistrationModule LinqToDbDataAccessModule(this IServiceCollection services)
            => services.Module<RegistrationModule>();

        public static void AddLinqToDbDataAccessModule(this IServiceCollection services)
            => services.AddModule<RegistrationModule>();

        public static void AddDataConnectionFactory(this IServiceCollection services, string name, Type dbContextType, Action<DataConnection> configurator = null)
        {
            configurator ??= db => { };

            services.AddScoped(typeof(IDataConnectionFactory<>).MakeGenericType(dbContextType),
                sp => ActivatorUtilities.CreateInstance(sp,
                    typeof(DataConnectionFactory<>).MakeGenericType(dbContextType), name, configurator));
        }

        public static void AddDataConnectionFactory<TDataConnection>(this IServiceCollection services, string name, Action<TDataConnection> configurator = null)
            where TDataConnection: DataConnection
        {
            configurator ??= db => { };

            services.AddDataConnectionFactory(name, typeof(TDataConnection), db => configurator((TDataConnection)db));
        }

        public static void AddLinqToDbRepository(this IServiceCollection services, Type entityType, Type repositoryType = null, Type dbContextType = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));

            if (repositoryType == null && dbContextType != null)
                repositoryType = typeof(LinqToDbCrudRepository<,>).MakeGenericType(dbContextType, entityType);

            if (repositoryType == null) throw new ArgumentNullException(nameof(repositoryType));

            services.AddRepository(entityType, repositoryType);

            if (dbContextType != null)
            {
                services.TryAddScoped(
                    typeof(IPrimaryKeyGetter<>).MakeGenericType(entityType),
                    typeof(LinqToDbPrimaryKeyGetter<,>).MakeGenericType(dbContextType, entityType));
            }

            services.TryAddScoped(typeof(IQueryableEnumerator<>).MakeGenericType(entityType),
                typeof(LinqToDbQueryableEnumerator<>).MakeGenericType(entityType));
        }
    }
}
