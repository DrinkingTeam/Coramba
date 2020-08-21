using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Coramba.DataAccess.Ef
{
    public static class AsyncQueryableExtensions
    {
        #region Any/All

        public static async Task<bool> AnyAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).AnyAsync();

        public static async Task<bool> AnyAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, bool>> predicate)
            => await (await queryTask).AnyAsync(predicate);

        public static async Task<bool> AllAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, bool>> predicate)
            => await (await queryTask).AnyAsync(predicate);

        #endregion

        #region CountAsync

        public static async Task<int> CountAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).CountAsync();

        public static async Task<int> CountAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, bool>> predicate)
            => await (await queryTask).CountAsync(predicate);

        public static async Task<long> LongCountAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).LongCountAsync();

        public static async Task<long> LongCountAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, bool>> predicate)
            => await (await queryTask).LongCountAsync(predicate);

        #endregion

        #region First/FirstOrDefault

        public static async Task<T> FirstAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).FirstAsync();

        public static async Task<T> FirstAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, bool>> predicate)
            => await (await queryTask).FirstAsync(predicate);

        public static async Task<T> FirstOrDefaultAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).FirstOrDefaultAsync();

        public static async Task<T> FirstOrDefaultAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, bool>> predicate)
            => await (await queryTask).FirstOrDefaultAsync(predicate);

        #endregion

        #region Last/LastOrDefault

        public static async Task<T> LastAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).LastAsync();

        public static async Task<T> LastAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, bool>> predicate)
            => await (await queryTask).LastAsync(predicate);

        public static async Task<T> LastOrDefaultAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).LastOrDefaultAsync();

        public static async Task<T> LastOrDefaultAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, bool>> predicate)
            => await (await queryTask).LastOrDefaultAsync(predicate);

        #endregion

        #region Single/SingleOrDefault

        public static async Task<T> SingleAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).SingleAsync();

        public static async Task<T> SingleAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, bool>> predicate)
            => await (await queryTask).SingleAsync(predicate);

        public static async Task<T> SingleOrDefaultAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).SingleOrDefaultAsync();

        public static async Task<T> SingleOrDefaultAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, bool>> predicate)
            => await (await queryTask).SingleOrDefaultAsync(predicate);

        #endregion

        #region Min

        public static async Task<T> MinAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).MinAsync();

        public static async Task<TResult> MinAsync<T, TResult>(this Task<IQueryable<T>> queryTask, Expression<Func<T, TResult>> selector)
            => await (await queryTask).MinAsync(selector);

        #endregion

        #region Max

        public static async Task<T> MaxAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).MaxAsync();

        public static async Task<TResult> MaxAsync<T, TResult>(this Task<IQueryable<T>> queryTask, Expression<Func<T, TResult>> selector)
            => await (await queryTask).MaxAsync(selector);

        #endregion

        #region Sum

        public static async Task<decimal> SumAsync<T>(this Task<IQueryable<decimal>> queryTask)
            => await (await queryTask).SumAsync();

        public static async Task<decimal> SumAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, decimal>> selector)
            => await (await queryTask).SumAsync(selector);

        public static async Task<decimal?> SumAsync<T>(this Task<IQueryable<decimal?>> queryTask)
            => await (await queryTask).SumAsync();

        public static async Task<decimal?> SumAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, decimal?>> selector)
            => await (await queryTask).SumAsync(selector);


        public static async Task<int> SumAsync<T>(this Task<IQueryable<int>> queryTask)
            => await (await queryTask).SumAsync();

        public static async Task<int> SumAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, int>> selector)
            => await (await queryTask).SumAsync(selector);

        public static async Task<int?> SumAsync<T>(this Task<IQueryable<int?>> queryTask)
            => await (await queryTask).SumAsync();

        public static async Task<int?> SumAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, int?>> selector)
            => await (await queryTask).SumAsync(selector);


        public static async Task<long> SumAsync<T>(this Task<IQueryable<long>> queryTask)
            => await (await queryTask).SumAsync();

        public static async Task<long> SumAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, long>> selector)
            => await (await queryTask).SumAsync(selector);

        public static async Task<long?> SumAsync<T>(this Task<IQueryable<long?>> queryTask)
            => await (await queryTask).SumAsync();

        public static async Task<long?> SumAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, long?>> selector)
            => await (await queryTask).SumAsync(selector);


        public static async Task<float> SumAsync<T>(this Task<IQueryable<float>> queryTask)
            => await (await queryTask).SumAsync();

        public static async Task<float> SumAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, float>> selector)
            => await (await queryTask).SumAsync(selector);

        public static async Task<float?> SumAsync<T>(this Task<IQueryable<float?>> queryTask)
            => await (await queryTask).SumAsync();

        public static async Task<float?> SumAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, float?>> selector)
            => await (await queryTask).SumAsync(selector);


        public static async Task<double> SumAsync<T>(this Task<IQueryable<double>> queryTask)
            => await (await queryTask).SumAsync();

        public static async Task<double> SumAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, double>> selector)
            => await (await queryTask).SumAsync(selector);

        public static async Task<double?> SumAsync<T>(this Task<IQueryable<double?>> queryTask)
            => await (await queryTask).SumAsync();

        public static async Task<double?> SumAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, double?>> selector)
            => await (await queryTask).SumAsync(selector);

        #endregion

        #region Average

        public static async Task<decimal> AverageAsync<T>(this Task<IQueryable<decimal>> queryTask)
            => await (await queryTask).AverageAsync();

        public static async Task<decimal> AverageAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, decimal>> selector)
            => await (await queryTask).AverageAsync(selector);

        public static async Task<decimal?> AverageAsync<T>(this Task<IQueryable<decimal?>> queryTask)
            => await (await queryTask).AverageAsync();

        public static async Task<decimal?> AverageAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, decimal?>> selector)
            => await (await queryTask).AverageAsync(selector);


        public static async Task<double> AverageAsync<T>(this Task<IQueryable<double>> queryTask)
            => await (await queryTask).AverageAsync();

        public static async Task<double> AverageAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, double>> selector)
            => await (await queryTask).AverageAsync(selector);

        public static async Task<double?> AverageAsync<T>(this Task<IQueryable<double?>> queryTask)
            => await (await queryTask).AverageAsync();

        public static async Task<double?> AverageAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, double?>> selector)
            => await (await queryTask).AverageAsync(selector);


        public static async Task<double> AverageAsync<T>(this Task<IQueryable<int>> queryTask)
            => await (await queryTask).AverageAsync();

        public static async Task<double> AverageAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, int>> selector)
            => await (await queryTask).AverageAsync(selector);

        public static async Task<double?> AverageAsync<T>(this Task<IQueryable<int?>> queryTask)
            => await (await queryTask).AverageAsync();

        public static async Task<double?> AverageAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, int?>> selector)
            => await (await queryTask).AverageAsync(selector);



        public static async Task<double> AverageAsync<T>(this Task<IQueryable<long>> queryTask)
            => await (await queryTask).AverageAsync();

        public static async Task<double> AverageAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, long>> selector)
            => await (await queryTask).AverageAsync(selector);

        public static async Task<double?> AverageAsync<T>(this Task<IQueryable<long?>> queryTask)
            => await (await queryTask).AverageAsync();

        public static async Task<double?> AverageAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, long?>> selector)
            => await (await queryTask).AverageAsync(selector);


        public static async Task<float> AverageAsync<T>(this Task<IQueryable<float>> queryTask)
            => await (await queryTask).AverageAsync();

        public static async Task<float> AverageAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, float>> selector)
            => await (await queryTask).AverageAsync(selector);

        public static async Task<float?> AverageAsync<T>(this Task<IQueryable<float?>> queryTask)
            => await (await queryTask).AverageAsync();

        public static async Task<float?> AverageAsync<T>(this Task<IQueryable<T>> queryTask, Expression<Func<T, float?>> selector)
            => await (await queryTask).AverageAsync(selector);

        #endregion

        #region Contains

        public static async Task<bool> ContainsAsync<T>(this Task<IQueryable<T>> queryTask, T item)
            => await (await queryTask).ContainsAsync(item);

        #endregion

        #region ToList

        public static async Task<List<T>> ToListAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).ToListAsync();

        public static async Task<IEnumerable<T>> ToEnumerableAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).ToListAsync();

        #endregion

        #region ToArray

        public static async Task<T[]> ToArrayAsync<T>(this Task<IQueryable<T>> queryTask)
            => await (await queryTask).ToArrayAsync();

        #endregion

        #region Include

        public static async Task<IIncludableQueryable<TEntity, TProperty>> Include<TEntity, TProperty>(
            [NotNull] this Task<IQueryable<TEntity>> queryTask,
            [NotNull] Expression<Func<TEntity, TProperty>> navigationPropertyPath)
            where TEntity : class
            => (await queryTask).Include(navigationPropertyPath);

        public static async Task<IIncludableQueryable<TEntity, TProperty>> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            [NotNull] this Task<IIncludableQueryable<TEntity, IEnumerable<TPreviousProperty>>> queryTask,
            [NotNull] Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            where TEntity : class
            => (await queryTask).ThenInclude(navigationPropertyPath);

        public static async Task<IIncludableQueryable<TEntity, TProperty>> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            [NotNull] this Task<IIncludableQueryable<TEntity, TPreviousProperty>> queryTask,
            [NotNull] Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            where TEntity : class
            => (await queryTask).ThenInclude(navigationPropertyPath);

        public static async Task<IQueryable<TEntity>> Include<TEntity>(
            [NotNull] this Task<IQueryable<TEntity>> queryTask,
            [NotNull] [NotParameterized] string navigationPropertyPath)
            where TEntity : class
            => (await queryTask).Include(navigationPropertyPath);

        public static async Task<IQueryable<TEntity>> ToQueryable<TEntity, TProperty>([NotNull] this Task<IIncludableQueryable<TEntity, TProperty>> queryTask)
            where TEntity : class
            => await queryTask;

        #endregion

        #region Load

        public static async Task Load<TSource>([NotNull] this Task<IQueryable<TSource>> queryTask)
            => (await queryTask).Load();

        public static async Task LoadAsync<TSource>([NotNull] this Task<IQueryable<TSource>> queryTask)
            => await (await queryTask).LoadAsync();

        #endregion

        #region ToDictionary

        public static async Task<Dictionary<TKey, TSource>> ToDictionaryAsync<TSource, TKey>(
            [NotNull] this Task<IQueryable<TSource>> queryTask,
            [NotNull] Func<TSource, TKey> keySelector)
            => await (await queryTask).ToDictionaryAsync(keySelector);

        public static async Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(
            [NotNull] this Task<IQueryable<TSource>> queryTask,
            [NotNull] Func<TSource, TKey> keySelector,
            [NotNull] Func<TSource, TElement> elementSelector)
            => await (await queryTask).ToDictionaryAsync(keySelector, elementSelector);

        #endregion

        #region ForEach

        public static async Task ForEachAsync<T>(
            [NotNull] this Task<IQueryable<T>> queryTask,
            [NotNull] Action<T> action)
            => await (await queryTask).ForEachAsync(action);

        #endregion

        #region AsyncEnumerable

        public static async Task<IAsyncEnumerable<TSource>> AsAsyncEnumerable<TSource>(
            [NotNull] this Task<IQueryable<TSource>> queryTask)
            => (await queryTask).AsAsyncEnumerable();

        #endregion
    }
}
