using System;
using Coramba.DependencyInjection.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DataAccess.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RepositoryAttribute : Attribute, IServiceRegistrationAttribute
    {
        public Type RepositoryType { get; }

        public RepositoryAttribute(Type repositoryType)
        {
            RepositoryType = repositoryType;
        }

        public virtual void Register(IServiceCollection services, Type type)
        {
            services.AddRepository(type, RepositoryType);
        }
    }
}
