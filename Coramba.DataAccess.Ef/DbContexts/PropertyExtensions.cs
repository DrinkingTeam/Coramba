using System.Reflection;
using Coramba.Common.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Coramba.DataAccess.Ef.DbContexts
{
    public static class PropertyExtensions
    {
        public static PropertyInfo GetPropertyInfo(this IProperty property)
            => property.DeclaringType.ClrType.GetPropertyCached(property.Name);
    }
}
