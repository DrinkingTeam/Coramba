using System.Threading.Tasks;

namespace Coramba.Core.Converters
{
    public interface IObjectConverter<TSource, TDestination>
    {
        Task<TDestination> ConvertAsync(TSource source, TDestination destination);
    }
}
