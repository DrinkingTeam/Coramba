using System.Threading.Tasks;

namespace Coramba.DataAccess.Common
{
    public interface IEntityBeforeInsertListener<T>
    {
        Task OnBeforeInsertAsync(T model);
    }
}
