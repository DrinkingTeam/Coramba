using System;
using Microsoft.Extensions.DependencyInjection;
using Coramba.DependencyInjection;
using Coramba.DependencyInjection.Annotations;

namespace Coramba.Core.Converters
{
    public class ObjectConverterAttribute : ServiceRegistrationAttribute
    {
        public ObjectConverterAttribute(Type implementationType)
            : base(typeof(IObjectConverter<,>), implementationType, ServiceLifetime.Scoped)
        {
        }
    }
}
