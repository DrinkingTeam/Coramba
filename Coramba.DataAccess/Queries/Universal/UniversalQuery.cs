using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Threading.Tasks;
using Coramba.Common;
using Coramba.DataAccess.Base;

namespace Coramba.DataAccess.Queries.Universal
{
    public class UniversalQuery<T>: IQuery<T, UniversalFilter>
    {
        private readonly IPrimaryKeyGetter<T> _primaryKeyGetter;

        public UniversalQuery(IPrimaryKeyGetter<T> primaryKeyGetter)
        {
            _primaryKeyGetter = primaryKeyGetter;
        }

        private IQueryable<T> FilterByIds(IQueryable<T> queryable, object[] ids)
        {
            var keyProperties = _primaryKeyGetter.Get()?.Cast<PropertyInfo>().ToList();
            if (keyProperties == null || keyProperties.Count == 0)
                return queryable;
            if (keyProperties.Count > 1)
                throw new NotSupportedException($"Type {typeof(T)} contains composite primary key");
            var keyProperty = keyProperties.Single();

            var whereClause = Enumerable
                .Range(0, ids.Length)
                .Select(i => $"{keyProperty.Name} == @{i}")
                .Flatten(" || ");

            return queryable.Where(whereClause, ids);
        }

        public Task<IQueryable<T>> QueryAsync(IQueryable<T> queryable, UniversalFilter filter)
        {
            if (filter.Ids != null && filter.Ids.Length > 0)
                queryable = FilterByIds(queryable, filter.Ids);

            if (filter.Offset.HasValue && filter.Offset.Value > 0)
                queryable = queryable.Skip(filter.Offset.Value - 1);

            if (filter.Limit.HasValue)
                queryable = queryable.Take(filter.Limit.Value);

            var whereCondition = filter.Condition?.GetWhereClause(0);
            if (whereCondition != null)
                queryable = queryable.Where(whereCondition.Text, whereCondition.Parameters.ToArray());

            return Task.FromResult(queryable);
        }
    }
}
