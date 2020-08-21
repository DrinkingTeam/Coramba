using System;
using System.Collections.Generic;

namespace Coramba.Common
{
    public class Cache<TKey, TValue>
    {
        private Func<TKey, TValue> ValueFactory { get; }
        private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();

        public Cache(Func<TKey, TValue> valueFactory)
        {
            ValueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
        }

        public TValue Get(TKey key)
        {
            if (_cache.TryGetValue(key, out var value))
                return value;
            value = ValueFactory(key);
            _cache.Add(key, value);
            return value;
        }
    }
}
