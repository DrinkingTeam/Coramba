using System.Threading.Tasks;
using Coramba.DataAccess.Repositories;

namespace Coramba.DataAccess.Common
{
    public interface IEntityNewListener<T>
    {
        Task OnNewAsync(T model, RepositoryOperationContext context);
    }
}
