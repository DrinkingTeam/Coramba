namespace Coramba.Core.Converters
{
    /// <summary>
    /// Antipattern. Made for easy of use IObjectConverter`2. Try to not use it.
    /// </summary>
    public interface IObjectConverterResolver
    {
        IObjectConverter<TSource, TDestination> Resolve<TSource, TDestination>();
    }
}
