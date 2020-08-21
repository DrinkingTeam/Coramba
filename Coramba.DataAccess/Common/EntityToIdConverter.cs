using System.Threading.Tasks;
using Coramba.Core.Converters;

namespace Coramba.DataAccess.Common
{
    public class EntityToIdConverter<TEntity, TId> : ImmutableObjectConverter<TEntity, TId>
        where TEntity : IEntity<TId>
    {
        protected override Task<TId> ConvertToAsync(TEntity source)
        {
            return Task.FromResult(source.Id);
        }
    }
}
