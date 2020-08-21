using System.Threading.Tasks;

namespace Coramba.Core.Converters
{
    public class ObjectConvertBuilder<TSource>
    {
        private readonly IObjectConverterResolver _objectConverterResolver;
        private readonly TSource _source;
        private readonly Task<TSource> _sourceTask;

        public ObjectConvertBuilder(IObjectConverterResolver objectConverterResolver, TSource source)
        {
            _objectConverterResolver = objectConverterResolver;
            _source = source;
        }

        public ObjectConvertBuilder(IObjectConverterResolver objectConverterResolver, Task<TSource> sourceTask)
        {
            _objectConverterResolver = objectConverterResolver;
            _sourceTask = sourceTask;
        }

        public async Task<TDestination> ToAsync<TDestination>(TDestination destination = default)
        {
            if (_sourceTask != null)
                return await new ObjectConvertBuilder<TSource, TDestination>(_objectConverterResolver.Resolve<TSource, TDestination>(), await _sourceTask)
                    .ToAsync(destination);

            return await new ObjectConvertBuilder<TSource, TDestination>(_objectConverterResolver.Resolve<TSource, TDestination>(), _source)
                .ToAsync(destination);
        }
    }
}
