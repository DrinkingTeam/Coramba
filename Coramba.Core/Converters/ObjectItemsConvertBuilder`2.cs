using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coramba.Core.Converters
{
    public class ObjectItemsConvertBuilder<TSource, TDestination>
    {
        private readonly IObjectConverter<TSource, TDestination> _objectConverter;
        private readonly IEnumerable<TSource> _source;
        private readonly Task<IEnumerable<TSource>> _sourceTask;

        public ObjectItemsConvertBuilder(IObjectConverter<TSource, TDestination> objectConverter, IEnumerable<TSource> source)
        {
            _objectConverter = objectConverter ?? throw new ArgumentNullException(nameof(objectConverter));
            _source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public ObjectItemsConvertBuilder(IObjectConverter<TSource, TDestination> objectConverter, Task<IEnumerable<TSource>> sourceTask)
        {
            _objectConverter = objectConverter ?? throw new ArgumentNullException(nameof(objectConverter));
            _sourceTask = sourceTask ?? throw new ArgumentNullException(nameof(sourceTask));
        }

        public async Task<IEnumerable<TDestination>> ToAsync()
        {
            var source = _source;
            if (_sourceTask != null)
                source = await _sourceTask;

            var result = new List<TDestination>();
            foreach (var item in source)
                result.Add(await _objectConverter.Convert(item).ToAsync());

            return result;
        }
    }
}
