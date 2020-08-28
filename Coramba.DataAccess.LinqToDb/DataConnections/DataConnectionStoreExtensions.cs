using System;
using System.Threading.Tasks;
using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.DataConnections
{
    public static class DataConnectionStoreExtensions
    {
        public static async Task<TResult> WithDataConnectionAsync<TDataConnection, TResult>(
            this IDataConnectionStore<TDataConnection> store,
            string name, Func<Task<TResult>> func)
            where TDataConnection : DataConnection
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

        public static Task WithDataConnectionAsync<TDataConnection>(
            this IDataConnectionStore<TDataConnection> store,
            string name, Func<Task> action)
            where TDataConnection : DataConnection
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return store.WithDataConnectionAsync<TDataConnection, object>(name, async () =>
            {
                await action();
                return null;
            });
        }

        public static async Task<TResult> WithDataConnectionAsync<TDataConnection, TResult>(
            this IDataConnectionStore<TDataConnection> store,
            TDataConnection dbContext, Func<Task<TResult>> func)
            where TDataConnection : DataConnection
        {
            if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));
            var name = Guid.NewGuid().ToString("N");
            store.Add(name, dbContext);
            try
            {
                return await store.WithDataConnectionAsync(name, func);
            }
            finally
            {
                store.Remove(name);
            }
        }

        public static Task WithDataConnectionAsync<TDataConnection>(
            this IDataConnectionStore<TDataConnection> store,
            TDataConnection dbContext, Func<Task> action)
            where TDataConnection : DataConnection
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            return store.WithDataConnectionAsync<TDataConnection, object>(dbContext, async () =>
            {
                await action();
                return null;
            });
        }
    }
}
