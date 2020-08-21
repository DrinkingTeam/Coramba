using System.Threading.Tasks;

namespace Coramba.DataAccess.Repositories
{
    public interface ICrudRepository<T> : IRepository<T>
    {
        Task<T> NewAsync();
        Task InsertAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
