using System;
using System.ComponentModel;
using Coramba.DataAccess.Repositories;

namespace Coramba.DataAccess.Annotations
{
    public class UtcNowDefaultValueAttribute : AbstractDefaultValueAttribute
    {
        public override object GetDefaultValue(object model, PropertyDescriptor property, RepositoryOperationContext context)
            => context.UtcNow ?? DateTime.UtcNow;
    }
}
