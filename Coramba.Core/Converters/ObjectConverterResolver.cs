using System;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.Core.Converters
{
    public class ObjectConverterResolver : IObjectConverterResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public ObjectConverterResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IObjectConverter<TSource, TDestination> Resolve<TSource, TDestination>()
            => _serviceProvider.GetRequiredService<IObjectConverter<TSource, TDestination>>();
    }
}