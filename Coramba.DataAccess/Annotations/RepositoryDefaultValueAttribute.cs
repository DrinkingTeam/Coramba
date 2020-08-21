using System.ComponentModel;
using Coramba.DataAccess.Repositories;

namespace Coramba.DataAccess.Annotations
{
    public class RepositoryDefaultValueAttribute: AbstractDefaultValueAttribute
    {
        public object Value { get; }

        public RepositoryDefaultValueAttribute(object value)
        {
            Value = value;
        }

        public override object GetDefaultValue(object model, PropertyDescriptor property, RepositoryOperationContext context)
            => Value;
    }
}
