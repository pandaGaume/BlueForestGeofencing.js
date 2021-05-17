using System;
using System.Collections.Generic;
using System.Text;

namespace IOfThings.Spatial.Geofencing
{
    public interface ICache<TKey, TValue>
    {
        ICacheEntry<TKey, TValue> CreateEntry(TKey key);
        void Remove(TKey key);
        bool TryGetValue(TKey key, out TValue value);

    }
}
