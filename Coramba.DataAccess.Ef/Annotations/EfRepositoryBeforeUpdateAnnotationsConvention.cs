using System.Reflection;
using System.Threading.Tasks;
using Coramba.Common;
using Coramba.DataAccess.Conventions;
using Coramba.DataAccess.Ef.DbContexts;
using Coramba.DataAccess.Ef.Repositories;
using Coramba.DataAccess.Repositories;

namespace Coramba.DataAccess.Ef.Annotations
{
    public class EfRepositoryBeforeUpdateAnnotationsConvention<T> : IRepositoryBeforeInsertConvention<T>, IRepositoryBeforeUpdateConvention<T>
    {
        public Task ApplyAsync(T entity, RepositoryOperationContext context)
        {
            var efContext = (EfRepositoryOperationContext)context;

            efContext
                .DbContext
                .Entry(entity)
                .Properties
                .ForEach(p =>
                {
                    var propertyInfo = p.Metadata.GetPropertyInfo();
                    var disableUpdate = propertyInfo.GetCustomAttribute<DisableUpdateAttribute>() != null;
                    if (disableUpdate)
                    {
                        p.IsModified = false;
                        p.CurrentValue = p.OriginalValue;
                    }
                });

            return Task.CompletedTask;
        }
    }
}
