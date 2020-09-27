using System;
using System.Linq;
using Coramba.Common.Reflection;
using Coramba.Core.Converters;
using Coramba.DataAccess.Base;
using Coramba.DataAccess.Common;
using Coramba.DataAccess.Conventions;
using Coramba.DataAccess.Ef.Annotations;
using Coramba.DataAccess.Ef.Base;
using Coramba.DataAccess.Ef.DbContexts;
using Coramba.DataAccess.Ef.Repositories;
using Coramba.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Coramba.DataAccess.Ef
{
    public static class RegistrationExtensions
    {
        public static RegistrationModule EfDataAccessModule(this IServiceCollection services)
            => services.Module<RegistrationModule>();

        public static void AddEfDataAccessModule(this IServiceCollection services)
            => services.AddModule<RegistrationModule>();

        public static void AddDbContextFactory(this IServiceCollection services, string name, Type dbContextType, Action<DbContext> configurator = null)
        {
            configurator ??= db => { };

            services.AddScoped(typeof(IDbContextFactory<>).MakeGenericType(dbContextType),
                sp => ActivatorUtilities.CreateInstance(sp,
                    typeof(DbContextFactory<>).MakeGenericType(dbContextType), name, configurator));
        }

        public static void AddDbContextFactory<TDbContext>(this IServiceCollection services, string name, Action<TDbContext> configurator = null)
            where TDbContext: DbContext
        {
            configurator ??= db => { };

            services.AddDbContextFactory(name, typeof(TDbContext), db => configurator((TDbContext)db));
        }

        public static void AddEfRepository(this IServiceCollection services, Type entityType, Type repositoryType = null, Type dbContextType = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));

            if (repositoryType == null && dbContextType != null)
                repositoryType = typeof(EfCrudRepository<,>).MakeGenericType(dbContextType, entityType);

            if (repositoryType == null) throw new ArgumentNullException(nameof(repositoryType));

            services.AddRepository(entityType, repositoryType);

            if (dbContextType != null)
            {
                services.TryAddScoped(
                    typeof(IPrimaryKeyGetter<>).MakeGenericType(entityType),
                    typeof(EfPrimaryKeyGetter<,>).MakeGenericType(dbContextType, entityType));
            }

            services.AddScoped(typeof(IRepositoryConvention<>).MakeGenericType(entityType),
                typeof(EfRepositoryBeforeUpdateAnnotationsConvention<>).MakeGenericType(entityType));

            services.TryAddScoped(typeof(IQueryableEnumerator<>).MakeGenericType(entityType),
                typeof(EfQueryableEnumerator<>).MakeGenericType(entityType));
        }
    }
}
