using System;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DependencyInjection.Annotations
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class AutoRegistrationAttribute: Attribute
    {
        public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Scoped;
    }
}
