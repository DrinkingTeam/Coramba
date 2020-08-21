using System;
using System.Collections.Concurrent;

namespace Coramba.Common
{
    public class ConcurrentCache<TKey, TValue>
    {
        public Func<TKey, TValue> ValueFactory { get; }
        private readonly ConcurrentDictionary<TKey, TValue> _cache = new ConcurrentDictionary<TKey, TValue>();

        public ConcurrentCache(Func<TKey, TValue> valueFactory)
        {
            ValueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        }

        public TValue Get(TKey key)
            => _cache.GetOrAdd(key, ValueFactory);
    }
}
