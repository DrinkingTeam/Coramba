using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Coramba.Common.Reflection;
using Coramba.DataAccess.Base;
using Coramba.DataAccess.LinqToDb.DataConnections;
using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.Base
{
    public class LinqToDbModelFinder<TDataConnection, TModel> : IModelFinder<TModel>
        where TDataConnection: DataConnection
        where TModel : class
    {
        public IDataConnectionGetter<TDataConnection> DbContextGetter { get; }

        public LinqToDbModelFinder(IDataConnectionGetter<TDataConnection> dbContextGetter)
        {
            DbContextGetter = dbContextGetter ?? throw new ArgumentNullException(nameof(dbContextGetter));
        }

        private IEnumerable<(PropertyInfo Property, object Id)> GetPrimaryKey(object id)
        {
            var primaryKeys = DbContextGetter.Get().GetPkProperties(typeof(TModel));

            if (primaryKeys.Length == 0)
                throw new NotSupportedException($"Type {typeof(TModel)} doesn't contain primary key");

            if (primaryKeys.Length > 1)
                throw new NotSupportedException($"Type {typeof(TModel)} contains composite primary key");

            var keyProperty = primaryKeys.Single();
            var memberInfo = keyProperty.MemberInfo;

            if (memberInfo == null)
                throw new Exception($"Key member {keyProperty.ColumnName} for {typeof(TModel)} is not found");

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
