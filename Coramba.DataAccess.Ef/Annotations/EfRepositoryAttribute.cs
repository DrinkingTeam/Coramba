using System;
using System.Reflection;
using Coramba.DependencyInjection.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DataAccess.Ef.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EfRepositoryAttribute : Attribute, IServiceRegistrationAttribute
    {
        public Type RepositoryType { get; }

        public EfRepositoryAttribute(Type repositoryType = null)
        {
            RepositoryType = repositoryType;
        }

        public virtual void Register(IServiceCollection services, Type type)
        {
            services.AddEfRepository(type, RepositoryType, type.GetCustomAttribute<DbContextAttribute>()?.ContextType);
        }
    }
}
