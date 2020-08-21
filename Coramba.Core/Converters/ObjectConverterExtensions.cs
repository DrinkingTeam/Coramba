using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coramba.Core.Converters
{
    public static class ObjectConverterExtensions
    {
        public static ObjectConvertBuilder<TSource> Convert<TSource>(this IObjectConverterResolver objectConverterResolver, TSource source)
            => new ObjectConvertBuilder<TSource>(objectConverterResolver, source);

        public static ObjectConvertBuilder<TSource> Convert<TSource>(this IObjectConverterResolver objectConverterResolver, Task<TSource> source)
            => new ObjectConvertBuilder<TSource>(objectConverterResolver, source);

        public static ObjectConvertBuilder<TSource, TDestination> Convert<TSource, TDestination>(this IObjectConverter<TSource, TDestination> objectConverter, TSource source)
            => new ObjectConvertBuilder<TSource, TDestination>(objectConverter, source);

        public static ObjectConvertBuilder<TSource, TDestination> Convert<TSource, TDestination>(this IObjectConverter<TSource, TDestination> objectConverter, Task<TSource> source)
            => new ObjectConvertBuilder<TSource, TDestination>(objectConverter, source);


        public static ObjectItemsConvertBuilder<TSource> ConvertItems<TSource>(this IEnumerable<TSource> source, IObjectConverterResolver objectConverterResolver)
            => new ObjectItemsConvertBuilder<TSource>(objectConverterResolver, source);

        public static ObjectItemsConvertBuilder<TSource> ConvertItems<TSource>(this Task<IEnumerable<TSource>> source, IObjectConverterResolver objectConverterResolver)
            => new ObjectItemsConvertBuilder<TSource>(objectConverterResolver, source);

        public static ObjectItemsConvertBuilder<TSource, TDestination> ConvertItems<TSource, TDestination>(this IEnumerable<TSource> source, IObjectConverter<TSource, TDestination> objectConverter)
            => new ObjectItemsConvertBuilder<TSource, TDestination>(objectConverter, source);

        public static ObjectItemsConvertBuilder<TSource, TDestination> ConvertItems<TSource, TDestination>(this Task<IEnumerable<TSource>> source, IObjectConverter<TSource, TDestination> objectConverter)
            => new ObjectItemsConvertBuilder<TSource, TDestination>(objectConverter, source);
    }
}
