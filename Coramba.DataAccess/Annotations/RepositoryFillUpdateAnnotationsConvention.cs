using System.Threading.Tasks;
using Coramba.DataAccess.Common;
using Coramba.DataAccess.Conventions;
using Coramba.DataAccess.Repositories;

namespace Coramba.DataAccess.Annotations
{
    public class RepositoryFillUpdateAnnotationsConvention<T> : IRepositoryBeforeUpdateConvention<T>
    {
        public async Task ApplyAsync(T entity, RepositoryOperationContext context)
        {
            RepositoryFillNewAnnotationsConvention<T>.Fill(entity, ActionFlags.Update, context);

            if (entity is IEntityBeforeUpdateListener<T> listener)
                await listener.OnBeforeUpdateAsync(entity);
        }
    }
}
