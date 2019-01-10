using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCache
{

    public interface ICache<TKey, TValue>
    {
        event EventHandler<KeyEvictedEventArgs<TKey,TValue>> KeyEvicted;
        /// <summary>
        /// Gets the value from the Cache for the given key.  If the key isn't in the cache, it will get it from the backing store
        /// </summary>
        /// <param name="key">The Key to get</param>
        /// <returns>Result will be null if key does not exist</returns>
        Task<TValue> GetValueAsync(TKey key);
        /// <summary>
        /// Sets the value associated with the key.  This will also invoke the backing store
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>Faulted task if there are any error during saving the value</returns>
        Task SetValueAsync(TKey key, TValue value);
        /// <summary>
        /// Removes the key from the cache, it does NOT remove it from the backing store.  KeyEvicted event will be fired
        /// </summary>
        /// <param name="key">key to evict</param>
        /// <returns>Faulted task if there are any error during evicting the key</returns>
        Task EvictKeyAsync(TKey key);
        /// <summary>
        /// Removes key from the cache AND backing store.   KeyEvicted event will be fired
        /// </summary>
        /// <param name="key">key to remove</param>
        /// <returns>Faulted task if there are any error during evicting the key</returns>
        Task RemoveKeyAsync(TKey key);
        /// <summary>
        /// Checks if the key is located in the Cache or Backing store
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if found, false otherwise</returns>
        Task<bool> ContainsKeyAsync(TKey key);
        /// <summary>
        /// Checks if the key is located in the Cache only.  The backing store is not checked
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if found, false otherwise</returns>
        Task<bool> ContainsKeyInCacheAsync(TKey key);
    }
}
