using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coramba.Common
{
    public class AsyncCache<TKey, TValue>
    {
        private Func<TKey, Task<TValue>> ValueFactory { get; }
        private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();

        public AsyncCache(Func<TKey, Task<TValue>> valueFactory)
        {
            ValueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        }

        public async Task<TValue> GetAsync(TKey key)
        {
            if (_cache.TryGetValue(key, out var value))
                return value;
            value = await ValueFactory(key);
            _cache.Add(key, value);
            return value;
        }
    }
}
