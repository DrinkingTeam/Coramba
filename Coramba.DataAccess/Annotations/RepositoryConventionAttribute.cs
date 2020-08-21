using System;
using Coramba.DataAccess.Conventions;
using Coramba.DependencyInjection.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DataAccess.Annotations
{
    public class RepositoryConventionAttribute: ServiceRegistrationAttribute
    {
        public RepositoryConventionAttribute(Type conventionType)
            : base(typeof(IRepositoryConvention<>), conventionType, ServiceLifetime.Scoped)
        {
            
        }
    }
}
