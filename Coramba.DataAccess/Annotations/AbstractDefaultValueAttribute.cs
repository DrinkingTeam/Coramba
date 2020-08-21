using System;
using System.ComponentModel;
using Coramba.DataAccess.Repositories;

namespace Coramba.DataAccess.Annotations
{
    public abstract class AbstractDefaultValueAttribute : Attribute, IDefaultValueGetter
    {
        public ActionFlags On { get; set; } = ActionFlags.New;

        public abstract object GetDefaultValue(object model, PropertyDescriptor property, RepositoryOperationContext context);

        public virtual object GetInsertDefaultValue(object model, PropertyDescriptor property, RepositoryOperationContext context)
            => GetDefaultValue(model, property, context);

        public virtual object GetUpdateDefaultValue(object model, PropertyDescriptor property, RepositoryOperationContext context)
            => GetDefaultValue(model, property, context);

        protected virtual object Convert(object value, PropertyDescriptor property, RepositoryOperationContext context)
        {
            var converter = property.Converter
                            ?? TypeDescriptor.GetConverter(property.PropertyType);

            if (converter.CanConvertTo(property.PropertyType))
                converter.ConvertTo(value, property.PropertyType);

            return value;
        }

        public bool TryGetValue(object model, ActionFlags actionFilter, PropertyDescriptor property, RepositoryOperationContext context, out object value)
        {
            value = null;

            if (On.HasFlag(actionFilter))
            {
                value = Convert(GetInsertDefaultValue(model, property, context), property, context);
                return true;
            }

            if (On.HasFlag(actionFilter))
            {
                value = Convert(GetUpdateDefaultValue(model, property, context), property, context);
                return true;
            }

            return false;
        }
    }
}
