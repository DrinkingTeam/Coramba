using System;
using System.Threading.Tasks;

namespace Coramba.Common
{
    public static class AsyncExtensions
    {
        public static async Task<TOut> Apply<TIn, TOut>(this Task<TIn> input, Func<TIn, TOut> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return action(await input);
        }

        public static async Task<TOut> ApplyAsync<TIn, TOut>(this Task<TIn> input, Func<TIn, Task<TOut>> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            return await action(await input);
        }
    }
}
