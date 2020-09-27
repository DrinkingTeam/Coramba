using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Coramba.Common.Reflection;

namespace Coramba.DataAccess.Base
{
    public class ModelFinder<TModel> : IModelFinder<TModel>
    {
        private readonly IPrimaryKeyGetter<TModel> _primaryKeyGetter;

        public ModelFinder(IPrimaryKeyGetter<TModel> primaryKeyGetter)
        {
            _primaryKeyGetter = primaryKeyGetter;
        }

        private IEnumerable<(PropertyInfo Property, object Id)> GetPrimaryKey(object id)
        {
            var primaryKeys = _primaryKeyGetter.Get().ToList();
            if (primaryKeys == null || primaryKeys.Count == 0)
                throw new Exception($"Type {typeof(TModel)} doesn't contain a primary key");

            if (primaryKeys.Count > 1)
                throw new NotSupportedException($"Type {typeof(TModel)} contains composite primary key");

            var memberInfo = primaryKeys.Single();

            if (memberInfo.GetReturnType() != id.GetType())
                throw new Exception($"{id.GetType()} is not a valid type for primary key of {typeof(TModel)}, excepted {memberInfo.GetReturnType()}");

            if (!(memberInfo is PropertyInfo propertyInfo))
                throw new Exception($"Primary key of {typeof(TModel)} must be a property");

            yield return (propertyInfo, id);
        }

        public Task<IQueryable<TModel>> FindAsync(IQueryable<TModel> queryable, object criteria)
        {
            if (criteria == null) throw new ArgumentNullException(nameof(criteria));

            // sample below is not using filter conventions
            // return await DbContext.Set<TModel>().FindAsync(id);

            var modelParameter = Expression.Parameter(typeof(TModel), "m");
            Expression body = null;

            foreach (var key in GetPrimaryKey(criteria))
            {
                var equalExpression = Expression.Equal(
                    Expression.Convert(Expression.MakeMemberAccess(modelParameter, key.Property), key.Property.PropertyType),
                    Expression.Constant(key.Id));

                body = body == null ? equalExpression : Expression.And(body, equalExpression);
            }

            var whereExpression = Expression.Lambda<Func<TModel, bool>>(body, modelParameter);

            return Task.FromResult(queryable.Where(whereExpression));
        }
    }
}
