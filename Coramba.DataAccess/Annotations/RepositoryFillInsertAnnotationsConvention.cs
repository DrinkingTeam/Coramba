using System.Threading.Tasks;
using Coramba.DataAccess.Common;
using Coramba.DataAccess.Conventions;
using Coramba.DataAccess.Repositories;

namespace Coramba.DataAccess.Annotations
{
    public class RepositoryFillInsertAnnotationsConvention<T> : IRepositoryBeforeInsertConvention<T>
    {
        public async Task ApplyAsync(T entity, RepositoryOperationContext context)
        {
            RepositoryFillNewAnnotationsConvention<T>.Fill(entity, ActionFlags.Insert, context);

            if (entity is IEntityBeforeInsertListener<T> listener)
                await listener.OnBeforeInsertAsync(entity);
        }
    }
}
