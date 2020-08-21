using System.Threading.Tasks;

namespace Coramba.DataAccess.Common
{
    public interface IEntityBeforeUpdateListener<T>
    {
        Task OnBeforeUpdateAsync(T model);
    }
}
