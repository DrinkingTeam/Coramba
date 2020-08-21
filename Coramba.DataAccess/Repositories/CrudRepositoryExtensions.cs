using System;
using System.Threading.Tasks;

namespace Coramba.DataAccess.Repositories
{
    public static class CrudRepositoryExtensions
    {
        public static async Task<T> NewAsync<T>(this ICrudRepository<T> repository, Action<T> action)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var result = await repository.NewAsync();
            action(result);

            return result;
        }

        public static async Task<T> InsertNewAsync<T>(this ICrudRepository<T> repository, Action<T> action)
        {
            if (repository == null) throw new ArgumentNullException(nameof(repository));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var result = await repository.NewAsync();
            action(result);

            await repository.InsertAsync(result);

            return result;
        }
    }
}
