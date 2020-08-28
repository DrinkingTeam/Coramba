using System;
using System.Reflection;
using LinqToDB.Mapping;

namespace Coramba.DataAccess.LinqToDb.DataConnections
{
    public static class MappingSchemaExtensions
    {
        public static void SetDefaultTableName(this IEntityChangeDescriptor entityDescriptor, Func<Type, string> getName)
        {
            var tableAttribute = entityDescriptor.TypeAccessor.Type.GetCustomAttribute<TableAttribute>();
            if (tableAttribute?.Name == null)
                entityDescriptor.TableName = getName(entityDescriptor.TypeAccessor.Type);
        }

        public static void SetDefaultColumnName(this IEntityChangeDescriptor entityDescriptor, Func<IColumnChangeDescriptor, string> getName)
        {
            foreach (var column in entityDescriptor.Columns)
            {
                var columnDescriptor = (ColumnDescriptor) column;
                var columnAttribute = columnDescriptor.MemberInfo.GetCustomAttribute<ColumnAttribute>();
                if (columnAttribute?.Name == null)
                    column.ColumnName = getName(column);
            }
        }
    }
}
