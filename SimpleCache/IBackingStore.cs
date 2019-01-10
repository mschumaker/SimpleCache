namespace SimpleCache
{
    public interface IBackingStore<TKey, TValue>
    {
        /// <summary>
        /// Gets the value from the backing store for the given key.  
        /// </summary>
        /// <param name="key">The Key to get</param>
        /// <returns>Result will be null if key does not exist</returns>
        TValue GetValue(TKey key);
        /// <summary>
        /// Sets the value associated with the key. 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetValue(TKey key, TValue value);
        /// <summary>
        /// Removes key from the backing store. 
        /// </summary>
        /// <param name="key">key to remove</param>
        void RemoveKey(TKey key);
        /// <summary>
        /// Checks if the key is located in the backing store
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if found, false otherwise</returns>
        bool ContainsKey(TKey key);
    }
}
