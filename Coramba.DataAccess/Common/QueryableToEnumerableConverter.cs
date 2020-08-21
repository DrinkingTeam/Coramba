using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coramba.Core.Converters;
using Coramba.DataAccess.Base;

namespace Coramba.DataAccess.Common
{
    public class QueryableToEnumerableConverter<T> : ImmutableObjectConverter<IQueryable<T>, IEnumerable<T>>
    {
        protected IQueryableEnumerator<T> QueryableEnumerator { get; }

        public QueryableToEnumerableConverter(IQueryableEnumerator<T> queryableEnumerator)
        {
            QueryableEnumerator = queryableEnumerator ?? throw new ArgumentNullException(nameof(queryableEnumerator));
        }

        protected override Task<IEnumerable<T>> ConvertToAsync(IQueryable<T> source)
        {
            return source.EnumerateAsync(QueryableEnumerator);
        }
    }
}
