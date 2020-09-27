using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coramba.Common;
using Coramba.Core.Converters;
using Coramba.DataAccess.Base;
using Coramba.DataAccess.Common;
using Coramba.DataAccess.Conventions;
using Coramba.DependencyInjection.Annotations;
using Coramba.DependencyInjection.Modules;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Coramba.DataAccess
{
    [AutoModule(DependsOn = new[]
    {
        typeof(Core.RegistrationModule)
    })]
    public class RegistrationModule : ModuleBuilder<RegistrationModule, RegistrationModule.ComponentInfo>
    {
        public class ComponentInfo
        {
            public List<Type> Types { get; } = new List<Type>();
        }

        public RegistrationModule EntitiesFromAssembly(Assembly assembly, Func<Type, bool> typeFilter)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (typeFilter == null) throw new ArgumentNullException(nameof(typeFilter));

            return WithComponent(c =>
            {
                assembly
                    .GetTypes()
                    .Where(t => !t.IsAbstract && t.IsPublic && !t.IsGenericTypeDefinition && t.IsClass)
                    .Where(typeFilter)
                    .ForEach(c.Types.Add);
            });
        }

        public RegistrationModule Entity<T>()
            => WithComponent(c => c.Types.Add(typeof(T)));

        public RegistrationModule Entities(params Type[] types)
            => WithComponent(c => c.Types.AddRange(types));

        protected override void Register()
        {
            Component
                .Types
                .ForEach(Services.AddEntity);
        }

        protected override void RegisterOnce()
        {
            Services.TryAddScoped(typeof(IRepositoryConventions<>), typeof(RepositoryConventions<>));
            Services.TryAddScoped<IObjectConverterResolver, ObjectConverterResolver>();
            Services.TryAddScoped(typeof(IQueryTransformer<,>), typeof(QueryTransformer<,>));
            Services.TryAddScoped(typeof(IModelFinder<>), typeof(ModelFinder<>));
        }
    }
}
