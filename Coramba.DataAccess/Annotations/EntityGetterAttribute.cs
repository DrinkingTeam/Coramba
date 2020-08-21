using System;
using Coramba.Core.Converters;

namespace Coramba.DataAccess.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityGetterAttribute : ObjectConverterAttribute
    {
        public EntityGetterAttribute(Type modelGetterType)
            : base(modelGetterType)
        {
            
        }
    }
}
