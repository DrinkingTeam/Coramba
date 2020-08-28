using System;
using System.Reflection;
using Coramba.DependencyInjection.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DataAccess.LinqToDb.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LinqToDbRepositoryAttribute : Attribute, IServiceRegistrationAttribute
    {
        public Type RepositoryType { get; }

        public LinqToDbRepositoryAttribute(Type repositoryType = null)
        {
            RepositoryType = repositoryType;
        }

        public virtual void Register(IServiceCollection services, Type type)
        {
            services.AddLinqToDbRepository(type, RepositoryType, type.GetCustomAttribute<DataConnectionAttribute>()?.DataConnectionType);
        }
    }
}
