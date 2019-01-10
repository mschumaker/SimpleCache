using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCache.Unbounded
{
    /// <summary>
    /// An unbounded cache, keys will never be removed from the cache, except by the user.  Cache will continue to grow as new keys are requested
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class UnboundedCache<TKey, TValue> : ICache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _cache;
        private readonly IBackingStore<TKey, TValue> _backingStore;
        private readonly TaskScheduler _taskScheduler;
        private readonly TaskFactory _taskFactory;

        public UnboundedCache(IBackingStore<TKey, TValue> backingStore, TaskScheduler taskScheduler = null)
        {
            _backingStore = backingStore;
            _taskScheduler = taskScheduler ?? TaskScheduler.Default;
            _taskFactory = new TaskFactory(_taskScheduler);
            _cache = new Dictionary<TKey, TValue>();
        }

        public event EventHandler<KeyEvictedEventArgs<TKey, TValue>> KeyEvicted;


        public Task<bool> ContainsKeyAsync(TKey key)
        {
            return _taskFactory.StartNew<bool>(() =>
            {
                lock (_cache)
                {
                    if (_cache.ContainsKey(key))
                    {
                        return true;
                    }
                }
                return _backingStore.ContainsKey(key);
            });
        }

        public Task<bool> ContainsKeyInCacheAsync(TKey key)
        {
            return _taskFactory.StartNew(() =>
            {
                lock (_cache)
                {
                    return _cache.ContainsKey(key);
                }
            });
        }

        public Task EvictKeyAsync(TKey key)
        {
            return _taskFactory.StartNew(() =>
            {
                lock (_cache)
                {
                    if (_cache.ContainsKey(key))
                    {
                        var value = _cache[key];
                        _cache.Remove(key);
                        KeyEvicted?.Invoke(this, new KeyEvictedEventArgs<TKey, TValue>(key, value));
                    } else
                    {
                        throw new KeyNotFoundException($"Missing Key:{key}");
                    }
                }
            });
        }

        public Task<TValue> GetValueAsync(TKey key)
        {
            return _taskFactory.StartNew(() =>
            {
                lock (_cache)
                {
                    if (_cache.ContainsKey(key)) return _cache[key];
                }
                var result = _backingStore.GetValue(key);
                lock (_cache)
                {
                    if (_cache.ContainsKey(key)) return _cache[key];
                    _cache[key] = result;
                }
                return result;
            });
        }

        public Task RemoveKeyAsync(TKey key)
        {            
            return _taskFactory.StartNew(() =>
            {
                lock (_cache)
                {
                    if (_cache.ContainsKey(key))
                    {
                        var value = _cache[key];
                        _backingStore.RemoveKey(key);
                        _cache.Remove(key);
                        KeyEvicted?.Invoke(this, new KeyEvictedEventArgs<TKey, TValue>(key, value));
                    }
                    else
                    {
                        _backingStore.RemoveKey(key);
                    }
                }
            });
        }

        public Task SetValueAsync(TKey key, TValue value)
        {
            return _taskFactory.StartNew(() =>
            {
                _backingStore.SetValue(key, value);
                lock (_cache)
                {
                    _cache[key] = value;
                }
            });
        }
    }
}
