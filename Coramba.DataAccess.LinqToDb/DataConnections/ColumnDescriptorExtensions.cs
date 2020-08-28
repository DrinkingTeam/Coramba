using System.Reflection;
using LinqToDB.Mapping;

namespace Coramba.DataAccess.LinqToDb.DataConnections
{
    public static class ColumnDescriptorExtensions
    {
        public static PropertyInfo GetPropertyInfo(this ColumnDescriptor column)
            => (PropertyInfo)column.MemberInfo;
    }
}
