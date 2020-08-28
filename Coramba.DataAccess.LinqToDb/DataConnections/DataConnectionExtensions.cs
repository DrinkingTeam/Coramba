using System;
using System.Linq;
using LinqToDB.Data;
using LinqToDB.Mapping;

namespace Coramba.DataAccess.LinqToDb.DataConnections
{
    public static class DataConnectionExtensions
    {
        public static ColumnDescriptor[] GetPkProperties(this DataConnection dataConnection, Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException(nameof(objectType));
            var pkProperties = dataConnection
                .MappingSchema
                .GetEntityDescriptor(objectType)
                .Columns
                .Where(c => c.IsPrimaryKey)
                .OrderBy(c => c.PrimaryKeyOrder)
                .ToArray();

            return pkProperties;
        }

        public static object[] GetValues(this DataConnection dataConnection, object entity, params ColumnDescriptor[] properties)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            return properties
                .Select(p => p.GetValue(entity))
                .ToArray();
        }

        public static object[] GetPkValues(this DataConnection dataConnection, object entity)
            => dataConnection.GetValues(entity, dataConnection.GetPkProperties(entity?.GetType()));
    }
}
