using System;
using System.ComponentModel;
using Coramba.DataAccess.Repositories;

namespace Coramba.DataAccess.Annotations
{
    public class GuidDefaultValueAttribute : AbstractDefaultValueAttribute
    {
        public override object GetDefaultValue(object model, PropertyDescriptor property, RepositoryOperationContext context)
            => Guid.NewGuid();
    }
}
