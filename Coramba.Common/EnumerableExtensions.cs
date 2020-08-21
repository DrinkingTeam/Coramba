using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coramba.Common
{
    public static class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in items)
                action(item);
        }

        public static async Task ForEachAsync<T>(this IEnumerable<T> items, Func<T, Task> action)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (action == null) throw new ArgumentNullException(nameof(action));

            foreach (var item in items)
                await action(item);
        }

        public static async Task<IEnumerable<TResult>> SelectAsync<T, TResult>(this IEnumerable<T> items, Func<T, Task<TResult>> action)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var result = new List<TResult>();
            foreach (var item in items)
                result.Add(await action(item));

            return result;
        }

        public static async Task<IEnumerable<T>> WhereAsync<T>(this IEnumerable<T> items, Func<T, Task<bool>> action)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (action == null) throw new ArgumentNullException(nameof(action));

            var result = new List<T>();
            foreach (var item in items)
                if (await action(item))
                    result.Add(item);

            return result;
        }

        public static bool TrySingleOrDefault<T>(this IEnumerable<T> items, out T result)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            var first = true;
            result = default;
            foreach (var item in items.Take(2))
            {
                if (!first)
                    return false;

                result = item;
                first = false;
            }

            return true;
        }

        public static IEnumerable<IEnumerable<T>> Slice<T>(this IEnumerable<T> list, Func<T, bool> shouldSlice, bool sliceBefore = true)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (shouldSlice == null) throw new ArgumentNullException(nameof(shouldSlice));

            var cur = new List<T>();
            foreach (var item in list)
            {
                if (!shouldSlice(item))
                {
                    cur.Add(item);
                    continue;
                }

                if (sliceBefore)
                {
                    yield return cur;
                    cur.Clear();
                    cur.Add(item);
                }
                else
                {
                    cur.Add(item);
                    yield return cur;
                    cur.Clear();
                }
            }
            if (cur.Count > 0)
                yield return cur;
        }

        public static IEnumerable<T> ModifyFirst<T>(this IEnumerable<T> items, Func<T, T> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            var first = true;
            foreach (var item in items)
            {
                if (!first)
                    yield return item;
                else
                    yield return func(item);
                first = false;
            }
        }

        public static IEnumerable<T> ModifyLast<T>(this IEnumerable<T> items, Func<T, T> func)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));
            var last = default(T);
            var first = true;
            foreach (var item in items)
            {
                if (!first)
                    yield return last;
                first = false;
                last = item;
            }
            if (!first)
                yield return func(last);
        }
    }
}
