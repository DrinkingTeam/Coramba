using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Coramba.DataAccess.Ef.Transactions
{
    public static class EfTransactionsExtensions
    {
        public static async Task<TResult> WithTransactionAsync<TDbContext, TResult>(this IEfTransactionFactory<TDbContext> factory, Func<IDbContextTransaction, Task<TResult>> action)
            where TDbContext : DbContext
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

        public static Task<TResult> WithTransactionAsync<TDbContext, TResult>(this IEfTransactionFactory<TDbContext> factory, Func<Task<TResult>> func)
            where TDbContext : DbContext
            => factory.WithTransactionAsync<TDbContext, TResult>(async t => await func());

        public static Task WithTransactionAsync<TDbContext>(this IEfTransactionFactory<TDbContext> factory, Func<IDbContextTransaction, Task> func)
            where TDbContext : DbContext
            => factory.WithTransactionAsync<TDbContext, object>(async t =>
            {
                await func(t);
                return null;
            });

        public static Task WithTransactionAsync<TDbContext>(this IEfTransactionFactory<TDbContext> factory, Func<Task> func)
            where TDbContext : DbContext
            => factory.WithTransactionAsync<TDbContext>(async t => await func());
    }
}
