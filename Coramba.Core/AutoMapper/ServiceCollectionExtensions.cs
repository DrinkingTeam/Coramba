using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.Core.AutoMapper
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAutoMapperWithAnnotations(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            serviceCollection.AddAutoMapper(e => e.AddByAnnotations(assemblies), assemblies);
        }
    }
}
