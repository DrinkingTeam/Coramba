using System;
using System.Threading.Tasks;

namespace Coramba.Core.Converters
{
    public class ObjectConvertBuilder<TSource, TDestination>
    {
        private readonly IObjectConverter<TSource, TDestination> _objectConverter;
        private readonly TSource _source;
        private readonly Task<TSource> _sourceTask;

        public ObjectConvertBuilder(IObjectConverter<TSource, TDestination> objectConverter, TSource source)
        {
            _objectConverter = objectConverter ?? throw new ArgumentNullException(nameof(objectConverter));
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public ObjectConvertBuilder(IObjectConverter<TSource, TDestination> objectConverter, Task<TSource> sourceTask)
        {
            _objectConverter = objectConverter ?? throw new ArgumentNullException(nameof(objectConverter));
            _sourceTask = sourceTask ?? throw new ArgumentNullException(nameof(sourceTask));
        }

        public async Task<TDestination> ToAsync(TDestination destination = default)
        {
            if (_sourceTask != null)
                return await _objectConverter.ConvertAsync(await _sourceTask, destination);

            return await _objectConverter.ConvertAsync(_source, destination);
        }
    }
}
