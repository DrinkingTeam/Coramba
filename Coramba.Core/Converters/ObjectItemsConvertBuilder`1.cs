using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coramba.Core.Converters
{
    public class ObjectItemsConvertBuilder<TSource>
    {
        private readonly IObjectConverterResolver _objectConverterResolver;
        private readonly IEnumerable<TSource> _source;
        private readonly Task<IEnumerable<TSource>> _sourceTask;

        public ObjectItemsConvertBuilder(IObjectConverterResolver objectConverterResolver, IEnumerable<TSource> source)
        {
            _objectConverterResolver = objectConverterResolver;
            _source = source;
        }

        public ObjectItemsConvertBuilder(IObjectConverterResolver objectConverterResolver, Task<IEnumerable<TSource>> sourceTask)
        {
            _objectConverterResolver = objectConverterResolver;
            _sourceTask = sourceTask;
        }

        public async Task<IEnumerable<TDestination>> ToAsync<TDestination>()
        {
            if (_sourceTask != null)
                return await new ObjectItemsConvertBuilder<TSource, TDestination>(_objectConverterResolver.Resolve<TSource, TDestination>(), await _sourceTask)
                    .ToAsync();

            return await new ObjectItemsConvertBuilder<TSource, TDestination>(_objectConverterResolver.Resolve<TSource, TDestination>(), _source)
                .ToAsync();
        }
    }
}
