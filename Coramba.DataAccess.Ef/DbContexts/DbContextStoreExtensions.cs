using System;
using System.Threading.Tasks;

namespace Coramba.DataAccess.Ef.DbContexts
{
    public static class DbContextStoreExtensions
    {
        public static async Task<TResult> WithDbContextAsync<TDbContext, TResult>(
            this IDbContextStore<TDbContext> store,
            string name, Func<Task<TResult>> func)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (func == null) throw new ArgumentNullException(nameof(func));

            var old = store.GetCurrent();
            try
            {
                store.SetCurrent(name);

                return await func();
            }
            finally
            {
                store.SetCurrent(old);
            }
        }

        public static Task WithDbContextAsync<TDbContext>(
            this IDbContextStore<TDbContext> store,
            string name, Func<Task> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return store.WithDbContextAsync<TDbContext, object>(name, async () =>
            {
                await action();
                return null;
            });
        }

        public static async Task<TResult> WithDbContextAsync<TDbContext, TResult>(
            this IDbContextStore<TDbContext> store,
            TDbContext dbContext, Func<Task<TResult>> func)
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            var name = Guid.NewGuid().ToString("N");
            store.Add(name, dbContext);
            try
            {
                return await store.WithDbContextAsync(name, func);
            }
            finally
            {
                store.Remove(name);
            }
        }

        public static Task WithDbContextAsync<TDbContext>(
            this IDbContextStore<TDbContext> store,
            TDbContext dbContext, Func<Task> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return store.WithDbContextAsync<TDbContext, object>(dbContext, async () =>
            {
                await action();
                return null;
            });
        }
    }
}
