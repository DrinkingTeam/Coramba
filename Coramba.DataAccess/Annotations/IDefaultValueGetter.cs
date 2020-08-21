using System.ComponentModel;
using Coramba.DataAccess.Repositories;

namespace Coramba.DataAccess.Annotations
{
    public interface IDefaultValueGetter
    {
        bool TryGetValue(object model, ActionFlags actionFilter, PropertyDescriptor property, RepositoryOperationContext context, out object value);
    }
}
