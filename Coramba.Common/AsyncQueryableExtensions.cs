using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Coramba.Common
{
    public static class AsyncQueryableExtensions
    {
        public static async Task<IQueryable<T>> Where<T>(this Task<IQueryable<T>> queueTask, Expression<Func<T, bool>> predicate)
            => (await queueTask).Where(predicate);

        public static async Task<T> First<T>(this Task<IQueryable<T>> queueTask)
            => (await queueTask).First();

        public static async Task<T> First<T>(this Task<IQueryable<T>> queueTask, Expression<Func<T, bool>> predicate)
            => (await queueTask).First(predicate);

        public static async Task<T> FirstOrDefault<T>(this Task<IQueryable<T>> queueTask)
            => (await queueTask).FirstOrDefault();

        public static async Task<T> FirstOrDefault<T>(this Task<IQueryable<T>> queueTask, Expression<Func<T, bool>> predicate)
            => (await queueTask).FirstOrDefault(predicate);

        public static async Task<T> Single<T>(this Task<IQueryable<T>> queueTask)
            => (await queueTask).Single();

        public static async Task<T> Single<T>(this Task<IQueryable<T>> queueTask, Expression<Func<T, bool>> predicate)
            => (await queueTask).Single(predicate);

        public static async Task<T> SingleOrDefault<T>(this Task<IQueryable<T>> queueTask)
            => (await queueTask).SingleOrDefault();

        public static async Task<T> SingleOrDefault<T>(this Task<IQueryable<T>> queueTask, Expression<Func<T, bool>> predicate)
            => (await queueTask).SingleOrDefault(predicate);

        public static async Task<IQueryable<TResult>> Select<TIn, TResult>(this Task<IQueryable<TIn>> queueTask, Expression<Func<TIn, TResult>> selector)
            => (await queueTask).Select(selector);

        public static async Task<IQueryable<TIn>> OrderBy<TIn, TKey>(this Task<IQueryable<TIn>> queueTask, Expression<Func<TIn, TKey>> keySelector)
            => (await queueTask).OrderBy(keySelector);

        public static async Task<IQueryable<TIn>> OrderByDescending<TIn, TKey>(this Task<IQueryable<TIn>> queueTask, Expression<Func<TIn, TKey>> keySelector)
            => (await queueTask).OrderByDescending(keySelector);

        public static async Task<IQueryable<T>> Take<T>(this Task<IQueryable<T>> queueTask, int count)
            => (await queueTask).Take(count);
    }
}
