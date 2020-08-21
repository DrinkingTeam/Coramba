using System;
using System.Linq;
using Coramba.Common.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Coramba.DataAccess.Ef.DbContexts
{
    public static class DbContextExtensions
    {
        public static IProperty[] GetPkProperties(this Microsoft.EntityFrameworkCore.DbContext dbContext, Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException(nameof(objectType));
            var pkProperties = dbContext
                .Model
                .FindEntityType(objectType)
                .FindPrimaryKey()
                ?.Properties;

            return pkProperties?.ToArray() ?? new IProperty[0];
        }

        public static object[] GetValues(this Microsoft.EntityFrameworkCore.DbContext dbContext, object entity, params IProperty[] properties)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            return properties
                .Select(p => entity.GetType().GetPropertyCached(p.Name))
                .Select(p => p == null ? null : p.GetValueFast(entity))
                .ToArray();
        }

        public static object[] GetPkValues(this Microsoft.EntityFrameworkCore.DbContext dbContext, object entity)
            => dbContext.GetValues(entity, dbContext.GetPkProperties(entity?.GetType()));
    }
}
