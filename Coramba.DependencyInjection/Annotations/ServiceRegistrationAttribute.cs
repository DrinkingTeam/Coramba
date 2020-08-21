using System;
using System.Collections.Generic;
using System.Linq;
using Coramba.Common.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DependencyInjection.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ServiceRegistrationAttribute: Attribute, IServiceRegistrationAttribute
    {
        protected ServiceLifetime ServiceLifetime { get; }

        protected Type[] ServiceTypes { get; }
        protected Type[][] ServiceTypeArguments { get; set; }

        protected Type ImplementationType { get; }
        protected Type[] ImplementationTypeArguments { get; set; }

        protected virtual Type[] GetServiceTypeArguments(Type serviceType, Type[] serviceTypeArguments, Type type, Type implementationType)
        {
            return serviceType.GetGenericTypeArguments(type, serviceType.GetGenericBaseTypeArguments(implementationType, serviceTypeArguments));
        }

        private Type BuildImplementationType(Type type)
        {
            var implementationType = ImplementationType;
            if (implementationType.IsGenericTypeDefinition)
                implementationType = implementationType.MakeGenericType(GetImplementationTypeArguments(type));
            return implementationType;
        }

        protected virtual Type[] GetImplementationTypeArguments(Type type)
            => ImplementationType.GetGenericTypeArguments(type, ImplementationTypeArguments);

        public ServiceRegistrationAttribute(Type serviceType, Type implementationType, ServiceLifetime serviceLifetime)
            : this(new[] { serviceType }, implementationType, serviceLifetime)
        {
        }

        public ServiceRegistrationAttribute(IEnumerable<Type> serviceTypes, Type implementationType, ServiceLifetime serviceLifetime)
        {
            if (serviceTypes == null) throw new ArgumentNullException(nameof(serviceTypes));

            ServiceTypes = serviceTypes.ToArray();

            if (ServiceTypes.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(serviceTypes));


            ImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
            ServiceLifetime = serviceLifetime;
        }

        protected virtual void AddToCollection(IServiceCollection services, List<Type> serviceTypes, Type implementationType)
        {
            services.AddMultiple(serviceTypes, implementationType, ServiceLifetime);
        }

        public virtual void Register(IServiceCollection services, Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            if (ServiceTypes == null)
                return;

            var implementationType = BuildImplementationType(type);

            var serviceTypeResult = new List<Type>();
            for (var index = 0; index < ServiceTypes.Length; index++)
            {
                var serviceType = ServiceTypes[index];
                if (serviceType.IsGenericTypeDefinition)
                    serviceType =
                        serviceType.MakeGenericType(
                            GetServiceTypeArguments(serviceType,
                                index < (ServiceTypeArguments?.Length).GetValueOrDefault() ? ServiceTypeArguments[index] : null,
                                type,
                                implementationType));

                serviceTypeResult.Add(serviceType);
            }

            AddToCollection(services, serviceTypeResult, implementationType);
        }
    }
}
