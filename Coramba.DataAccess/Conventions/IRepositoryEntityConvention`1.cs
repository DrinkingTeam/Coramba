using System.Threading.Tasks;
using Coramba.DataAccess.Repositories;

namespace Coramba.DataAccess.Conventions
{
    public interface IRepositoryModelConvention<T>: IRepositoryConvention<T>
    {
        Task ApplyAsync(T entity, RepositoryOperationContext context);
    }
}
