using System.Linq;
using System.Threading.Tasks;

namespace Coramba.DataAccess.Repositories
{
    public interface IRepository<T>
    {
        Task<IQueryable<T>> QueryAsync();
    }
}
