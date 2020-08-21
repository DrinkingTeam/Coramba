using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Coramba.Common;
using Coramba.DataAccess.Ef.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Coramba.DataAccess.Ef.DbContexts
{
    public static class ModelBuilderExtensions
    {
        public static void AddEntitiesFromAssembly(this ModelBuilder modelBuilder, Type dbContextType, Assembly assembly)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            if (dbContextType == null) throw new ArgumentNullException(nameof(dbContextType));
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            
            assembly
                .GetTypes()
                .Where(t => !t.IsAbstract)
                .Select(t => new
                {
                    Type = t,
                    DbContextType = t.GetCustomAttribute<DbContextAttribute>()?.ContextType
                })
                .Where(x => x.DbContextType == dbContextType)
                .ForEach(x =>
                {
                    modelBuilder.Entity(x.Type);
                });

            modelBuilder.ForAllProperties(p =>
            {
                var propertyInfo = p.GetPropertyInfo();
                var valueGenerated = propertyInfo.GetCustomAttribute<ValueGeneratedAttribute>()?.ValueGenerated ??
                                     ValueGenerated.Never;
                if (valueGenerated != ValueGenerated.Never)
                    p.ValueGenerated = valueGenerated;
            });
        }

        public static void ForAllTables(this ModelBuilder modelBuilder, Action<IMutableEntityType> action)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            if (action == null) throw new ArgumentNullException(nameof(action));

            modelBuilder
                .Model
                .GetEntityTypes()
                .ForEach(action);
        }

        public static void ForAllProperties(this ModelBuilder modelBuilder, Action<IMutableProperty> action)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            if (action == null) throw new ArgumentNullException(nameof(action));

            modelBuilder
                .ForAllTables(t =>
                {
                    t
                        .GetProperties()
                        .Where(p => !p.IsShadowProperty())
                        .ForEach(action);
                });
        }

        public static void SetDefaultColumnName(this ModelBuilder modelBuilder,
            Func<IMutableProperty, string> getColumnName)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            if (getColumnName == null) throw new ArgumentNullException(nameof(getColumnName));

            modelBuilder.ForAllProperties(p =>
            {
                var propertyInfo = p.GetPropertyInfo();
                if (p == null)
                    return;
                var name = propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name;
                if (name == null)
                    p.SetColumnName(getColumnName(p));
            });
        }

        public static void SetDefaultTableName(this ModelBuilder modelBuilder,
            Func<IMutableEntityType, string> getTableName)
        {
            if (modelBuilder == null) throw new ArgumentNullException(nameof(modelBuilder));
            if (getTableName == null) throw new ArgumentNullException(nameof(getTableName));

            modelBuilder.ForAllTables(t =>
            {
                var name = t.ClrType.GetCustomAttribute<TableAttribute>()?.Name;
                if (name == null)
                    t.SetTableName(getTableName(t));
            });
        }
    }
}
