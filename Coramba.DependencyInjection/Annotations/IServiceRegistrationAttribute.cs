using System;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DependencyInjection.Annotations
{
    public interface IServiceRegistrationAttribute
    {
        void Register(IServiceCollection services, Type type);
    }
}
