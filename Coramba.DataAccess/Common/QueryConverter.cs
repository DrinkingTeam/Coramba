using System.Collections.Generic;
using System.Linq;
using Coramba.Core.Converters;

namespace Coramba.DataAccess.Common
{
    public abstract class QueryConverter<TModel>: ImmutableObjectConverter<IQueryable<TModel>, IEnumerable<TModel>>
    {
    }
}
