using System;
using System.Collections.Generic;
using System.Linq;
using Coramba.Common;
using Coramba.Common.Reflection;
using Coramba.Core.Converters;
using Coramba.DataAccess.Annotations;
using Coramba.DataAccess.Common;
using Coramba.DataAccess.Conventions;
using Coramba.DataAccess.Queries;
using Coramba.DataAccess.Queries.Universal;
using Coramba.DataAccess.Repositories;
using Coramba.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Coramba.DataAccess
{
    public static class RegistrationExtensions
    {
        public static RegistrationModule DataAccessModule(this IServiceCollection services)
            => services.Module<RegistrationModule>();

        public static void AddDataAccessModule(this IServiceCollection services)
            => services.AddModule<RegistrationModule>();

        public static void AddEntity(this IServiceCollection services, Type entityType)
        {
            services.AddFromAnnotations(entityType);
        }

        public static void AddEntities(this IServiceCollection services, params Type[] entityTypes)
            => entityTypes.ForEach(services.AddEntity);

        public static void AddEntity<TEntity>(this IServiceCollection services)
        {
            services.AddEntity(typeof(TEntity));
        }

        private static IEnumerable<Type> GetAbstractRepositoryTypes(Type entityType, Type repositoryType)
        {
            var abstractRepositoryType = typeof(IRepository<>).MakeGenericType(entityType);

            foreach (var type in repositoryType.GetInterfaces().Where(i => abstractRepositoryType.IsAssignableFrom(i)))
            {
                yield return type;
            }
        }

        public static void AddRepository(this IServiceCollection services, Type entityType, Type repositoryType)
        {
            if (entityType == null) throw new ArgumentNullException(nameof(entityType));
            if (repositoryType == null) throw new ArgumentNullException(nameof(repositoryType));

            if (repositoryType.IsGenericTypeDefinition)
                repositoryType = repositoryType.MakeGenericType(repositoryType.GetGenericTypeArguments(entityType));

            var abstractRepositoryTypes = GetAbstractRepositoryTypes(entityType, repositoryType);

            services.AddScopedSmart(abstractRepositoryTypes, repositoryType, entityType);

            services.AddScoped(typeof(IRepositoryConvention<>).MakeGenericType(entityType),
                typeof(RepositoryFillNewAnnotationsConvention<>).MakeGenericType(entityType));

            services.AddScoped(typeof(IRepositoryConvention<>).MakeGenericType(entityType),
                typeof(RepositoryFillInsertAnnotationsConvention<>).MakeGenericType(entityType));

            services.AddScoped(typeof(IRepositoryConvention<>).MakeGenericType(entityType),
                typeof(RepositoryFillUpdateAnnotationsConvention<>).MakeGenericType(entityType));

            services.TryAddScoped(
                typeof(IObjectConverter<,>).MakeGenericType(typeof(IQueryable<>).MakeGenericType(entityType), typeof(IEnumerable<>).MakeGenericType(entityType)),
                    typeof(QueryableToEnumerableConverter<>).MakeGenericType(entityType));

            services.TryAddScoped(typeof(IQuery<,>).MakeGenericType(entityType, typeof(UniversalFilter)),
                typeof(UniversalQuery<>).MakeGenericType(entityType));

            entityType
                .GetImplementationTypes(typeof(IEntity<>))
                .ForEach(t =>
                {
                    var idType = t.GetGenericArguments().Single();

                    services.TryAddScoped(typeof(IObjectConverter<,>).MakeGenericType(entityType, idType),
                        typeof(EntityToIdConverter<,>).MakeGenericType(entityType, idType));
                });
        }
    }
}
