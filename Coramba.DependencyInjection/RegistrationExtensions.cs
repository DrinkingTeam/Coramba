using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coramba.Common;
using Coramba.Common.Reflection;
using Coramba.DependencyInjection.Annotations;
using Coramba.DependencyInjection.Modules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Coramba.DependencyInjection
{
    public static class RegistrationExtensions
    {
        public static RegistrationModule DependencyInjectionModule(this IServiceCollection services)
            => services.Module<RegistrationModule>();

        public static void AddDependencyInjectionModule(this IServiceCollection services)
            => services.AddModule<RegistrationModule>();

        #region Modules

        internal static void AddModule(IModule module, bool includeOncePart)
        {
            if (module == null) throw new ArgumentNullException(nameof(module));
            module.AddRegistration(includeOncePart);
        }

        public static void AddModule(this IServiceCollection services, Type moduleType, bool includeOncePart = false)
            => AddModule(services.Module(moduleType), includeOncePart);

        public static void AddModule<TModule>(this IServiceCollection services, bool includeOncePart = false)
            where TModule : IModule
            => AddModule(services.Module(typeof(TModule)), includeOncePart);

        public static TModule Module<TModule>(this IServiceCollection services)
            where TModule : IModule
            => (TModule)Module(services, typeof(TModule));

        public static IModule Module(this IServiceCollection services, Type moduleType)
        {
            IModule module;
            try
            {
                module = (IModule)Activator.CreateInstance(moduleType);
            }
            catch (Exception e)
            {
                throw new Exception($"Error while creating instance of type {moduleType}", e);
            }
            module.Init(services);
            return module;
        }

        private static void WithAutoModulesFromAssemblies(Assembly[] assemblies, Action<Type> moduleAction)
        {
            var assembliesHash = new HashSet<Assembly>(assemblies);

            AutoModuleWalker
                .GetModules(assembliesHash)
                .ForEach(moduleAction);
        }

        public static void AddAutoModules(this IServiceCollection services, params Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            if (assemblies.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(assemblies));

            WithAutoModulesFromAssemblies(assemblies, t => services.AddModule(t, true));
        }

        #endregion

        public static void AddFromAnnotations(this IServiceCollection services, Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            var attributes = type.GetCustomAttributes().OfType<IServiceRegistrationAttribute>();
            attributes.ForEach(a => a.Register(services, type));
        }

        public static void AddMultiple(this IServiceCollection services,
            IEnumerable<Type> serviceTypes,
            Type implementationType,
            ServiceLifetime lifetime)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (serviceTypes == null) throw new ArgumentNullException(nameof(serviceTypes));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));

            var servicesList = serviceTypes.ToList();
            if (!servicesList.Any())
                throw new InvalidOperationException($"{nameof(serviceTypes)} must be non empty");

            if (servicesList.Any(x => x == null))
                throw new InvalidOperationException($"{nameof(serviceTypes)} must contain non empty elements");

            if (servicesList.Any(x => !x.IsAssignableFrom(implementationType)))
                throw new Exception($"Service must be assignable from {implementationType}");

            Type baseServiceType = null;
            foreach (var serviceType in servicesList)
            {
                if (baseServiceType == null)
                {
                    services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
                    baseServiceType = serviceType;
                }
                else
                    services.Add(new ServiceDescriptor(serviceType, sp => sp.GetRequiredService(baseServiceType), lifetime));
            }
        }

        public static bool IsAdded(this IServiceCollection services, Type serviceType)
            => services.Any(sd => sd.ServiceType == serviceType);

        public static bool IsAdded<TService>(this IServiceCollection services)
            => services.IsAdded(typeof(TService));

        public static bool IsImplementationAdded(this IServiceCollection services, Type implementationType)
            => services.Any(sd => sd.ImplementationType == implementationType);

        public static bool IsImplementationAdded<TImplementation>(this IServiceCollection services)
            => services.IsImplementationAdded(typeof(TImplementation));

        public static bool HasMarker(this IServiceCollection services, Type serviceType)
            => services.IsAdded(typeof(ServiceRegistrationMarker<>).MakeGenericType(serviceType));

        public static bool HasMarker<TService>(this IServiceCollection services)
            => services.HasMarker(typeof(TService));

        public static void SetMarker(this IServiceCollection services, Type serviceType)
            => services.AddSingleton(typeof(ServiceRegistrationMarker<>).MakeGenericType(serviceType));

        public static void SetMarker<TService>(this IServiceCollection services)
            => services.SetMarker(typeof(TService));

        public static void AddAutoRegistrations(this IServiceCollection services, params Assembly[] assemblies)
        {
            var serviceTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttribute<AutoRegistrationAttribute>() != null)
                .ToHashSet();

            serviceTypes
                .ForEach(t =>
                {
                    if (t.IsGenericType)
                        throw new NotSupportedException($"Generic type auto registration is not supported: {t}");
                });

            assemblies
                .SelectMany(a => a.GetTypes())
                .Select(t =>
                {
                    var st = t.GetInterfaces().Where(i => serviceTypes.Contains(i)).Take(2).ToArray();
                    if (st.Length > 1)
                        throw new Exception($"Service type {t} should have only one implementation");

                    return new
                    {
                        ServiceType = st.FirstOrDefault(),
                        ImplementationType = t
                    };
                })
                .Where(x => x.ServiceType != null && x.ImplementationType != null)
                .ForEach(x =>
                {
                    services.TryAdd(new ServiceDescriptor(
                        x.ServiceType,
                        x.ImplementationType,
                        x.ServiceType.GetCustomAttribute<AutoRegistrationAttribute>().Lifetime));
                });
        }


        private static void AddSmart(this IServiceCollection services,
            IEnumerable<Type> serviceTypes, Type implementationType,
            Type defaultArgumentType, ServiceLifetime lifetime)
        {
            if (implementationType.IsGenericTypeDefinition)
                throw new ArgumentException($"Type {implementationType} can't be a generic type definition",
                    nameof(implementationType));

            var buildServiceTypes = new List<Type>();
            foreach (var serviceType in serviceTypes)
            {
                Type buildServiceType;

                if (!serviceType.IsGenericTypeDefinition)
                    buildServiceType = serviceType;
                else
                {
                    buildServiceType = serviceType.MakeGenericType(
                        serviceType.GetGenericTypeArguments(defaultArgumentType,
                            serviceType.GetGenericBaseTypeArguments(implementationType)));
                }

                buildServiceTypes.Add(buildServiceType);
            }

            services.AddMultiple(buildServiceTypes, implementationType, lifetime);
        }

        public static void AddScopedSmart(this IServiceCollection services, IEnumerable<Type> serviceTypes, Type implementationType, Type defaultArgumentType)
            => AddSmart(services, serviceTypes, implementationType, defaultArgumentType, ServiceLifetime.Scoped);
    }
}
