using System;
using System.Threading.Tasks;
using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.Transactions
{
    public static class LinqToDbTransactionsExtensions
    {
        public static async Task<TResult> WithTransactionAsync<TDataConnection, TResult>(this ILinqToDbTransactionFactory<TDataConnection> factory, Func<LinqToDbTransaction, Task<TResult>> action)
            where TDataConnection : DataConnection
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            var t = factory.Current;

            if (t != null)
                return await action(t);

            await using (t = await factory.CreateAsync())
            {
                var result = await action(t);
                await t.CommitAsync();
                return result;
            }
        }

        public static Task<TResult> WithTransactionAsync<TDataConnection, TResult>(this ILinqToDbTransactionFactory<TDataConnection> factory, Func<Task<TResult>> func)
            where TDataConnection : DataConnection
            => factory.WithTransactionAsync(async t => await func());

        public static Task WithTransactionAsync<TDataConnection>(this ILinqToDbTransactionFactory<TDataConnection> factory, Func<LinqToDbTransaction, Task> func)
            where TDataConnection : DataConnection
            => factory.WithTransactionAsync<TDataConnection, object>(async t =>
            {
                await func(t);
                return null;
            });

        public static Task WithTransactionAsync<TDataConnection>(this ILinqToDbTransactionFactory<TDataConnection> factory, Func<Task> func)
            where TDataConnection : DataConnection
            => factory.WithTransactionAsync(async t => await func());
    }
}
