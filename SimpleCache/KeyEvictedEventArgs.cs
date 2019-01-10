using System;

namespace SimpleCache
{
    public class KeyEvictedEventArgs<TKey, TValue> : EventArgs
    {
        public KeyEvictedEventArgs(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; }
        public TValue Value { get; }
    }
}
