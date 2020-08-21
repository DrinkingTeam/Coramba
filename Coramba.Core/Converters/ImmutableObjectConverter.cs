using System;
using System.Threading.Tasks;

namespace Coramba.Core.Converters
{
    public abstract class ImmutableObjectConverter<TSource, TDestination> : IObjectConverter<TSource, TDestination>
    {
        public async Task<TDestination> ConvertAsync(TSource id, TDestination destination)
        {
            if (!Equals(destination, default(TDestination)))
                throw new Exception($"Destination parameter must be set to default value");

            return await ConvertToAsync(id);
        }

        protected abstract Task<TDestination> ConvertToAsync(TSource source);
    }
}
