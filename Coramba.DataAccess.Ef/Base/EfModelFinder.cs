using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Coramba.Common.Reflection;
using Coramba.Core.Converters;
using Coramba.DataAccess.Base;
using Coramba.DataAccess.Ef.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Coramba.DataAccess.Ef.Base
{
    public class EfModelFinder<TDbContext, TModel> : IModelFinder<TModel>
        where TDbContext: DbContext
        where TModel : class
    {
        public IDbContextGetter<TDbContext> DbContextGetter { get; }

        public EfModelFinder(IDbContextGetter<TDbContext> dbContextGetter, IObjectConverter<IQueryable<TModel>, IEnumerable<TModel>> queryableToEnumerableConverter)
        {
            DbContextGetter = dbContextGetter ?? throw new ArgumentNullException(nameof(dbContextGetter));
        }

        private IEnumerable<(PropertyInfo Property, object Id)> GetPrimaryKey(object id)
        {
            var entityType = DbContextGetter.Get().Model.FindEntityType(typeof(TModel));
            if (entityType == null)
                throw new Exception($"Type {typeof(TModel)} not in DbContext");
            var primaryKey = entityType.FindPrimaryKey();
            if (entityType == null)
                throw new Exception($"Type {typeof(TModel)} doesn't contain a primary key");

            if (primaryKey.Properties.Count == 0)
                throw new NotSupportedException($"Type {typeof(TModel)} doesn't contain primary key");

            if (primaryKey.Properties.Count > 1)
                throw new NotSupportedException($"Type {typeof(TModel)} contains composite primary key");

            var keyProperty = primaryKey.Properties.Single();
            var memberInfo = typeof(TModel).GetPropertyCached(keyProperty.Name);

            if (memberInfo == null)
                throw new Exception($"Key member {keyProperty.Name} for {typeof(TModel)} is not found");

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
