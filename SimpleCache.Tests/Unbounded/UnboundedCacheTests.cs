using Moq;
using SimpleCache.Tests.Utils;
using SimpleCache.Unbounded;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SimpleCache.Tests.Unbounded
{
    public class UnboundedCacheSpec : Spec<UnboundedCache<String,int>>
    {
        protected const string Key = "key";
        protected Mock<IBackingStore<String, int>> BackingStore = new Mock<IBackingStore<string, int>>();
        protected CurrentThreadTaskScheduler TaskScheduler = new CurrentThreadTaskScheduler();

        public UnboundedCacheSpec()
        {
            Subject = new UnboundedCache<string, int>(BackingStore.Object, TaskScheduler);
        }
    }

    public class WhenConstructingCache : UnboundedCacheSpec
    {
        public WhenConstructingCache()
        {
            It = () => Assert.NotNull(Subject);
        }
    }
    public class UnboundedCacheTests
    {
        private const string Key = "key";

        [Fact]
        public void WhenConstructing()
        {
            var backingStoreMock = new Mock<IBackingStore<string, int>>();
            var cache = new UnboundedCache<string, int>(backingStoreMock.Object);
            Assert.NotNull(cache);
        }

        [Fact]
        public void WhenContainsKeyInCacheAsyncMissingKey()
        {
            var backingStoreMock = new Mock<IBackingStore<string, int>>();                
            var cache = new UnboundedCache<string, int>(backingStoreMock.Object,new CurrentThreadTaskScheduler());
            Assert.False(cache.ContainsKeyInCacheAsync(Key).Result);
        }

        [Fact]
        public void WhenContainsKeyInCacheAsyncKey()
        {
            var backingStoreMock = new Mock<IBackingStore<string, int>>();
            var cache = new UnboundedCache<string, int>(backingStoreMock.Object, new CurrentThreadTaskScheduler());
            cache.SetValueAsync(Key, 3);
            Assert.True(cache.ContainsKeyInCacheAsync(Key).Result);
        }

        [Fact]
        public void WhenSetValueAsync()
        {
            var backingStoreMock = new Mock<IBackingStore<string, int>>();
            var cache = new UnboundedCache<string, int>(backingStoreMock.Object, new CurrentThreadTaskScheduler());
            cache.SetValueAsync(Key, 3);
            Assert.True(cache.ContainsKeyInCacheAsync(Key).Result);
            backingStoreMock.Verify(f => f.SetValue(Key,3),Times.Once);
        }

        [Fact]
        public void WhenGetValueAsync()
        {
            var backingStoreMock = new Mock<IBackingStore<string, int>>();
            backingStoreMock.Setup(f => f.GetValue(Key)).Returns(3);
            var cache = new UnboundedCache<string, int>(backingStoreMock.Object, new CurrentThreadTaskScheduler());
            var result = cache.GetValueAsync(Key).Result;
            Assert.True(cache.ContainsKeyInCacheAsync(Key).Result);
            backingStoreMock.Verify(f => f.GetValue(Key), Times.Once);
            Assert.Equal(3, result);
        }

        [Fact]
        public void WhenEvictKeyAsync()
        {
            var backingStoreMock = new Mock<IBackingStore<string, int>>();
            var cache = new UnboundedCache<string, int>(backingStoreMock.Object, new CurrentThreadTaskScheduler());
            var evicted = false;
            cache.KeyEvicted += (s, a) => evicted = a.Key == Key;
            cache.SetValueAsync(Key, 3);
            Assert.True(cache.ContainsKeyInCacheAsync(Key).Result);

            var task = cache.EvictKeyAsync(Key);
            Assert.False(task.IsFaulted);
            Assert.False(cache.ContainsKeyInCacheAsync(Key).Result);
            Assert.True(evicted);
        }

        [Fact]
        public void WhenRemoveKeyAsync()
        {
            var backingStoreMock = new Mock<IBackingStore<string, int>>();
            var cache = new UnboundedCache<string, int>(backingStoreMock.Object, new CurrentThreadTaskScheduler());
            var evicted = false;
            cache.KeyEvicted += (s, a) => evicted = a.Key == Key;
            cache.SetValueAsync(Key, 3);
            Assert.True(cache.ContainsKeyInCacheAsync(Key).Result);

            var task = cache.RemoveKeyAsync(Key);
            backingStoreMock.Verify(f => f.RemoveKey(Key), Times.Once);
            Assert.False(task.IsFaulted);
            Assert.False(cache.ContainsKeyInCacheAsync(Key).Result);
            Assert.False(cache.ContainsKeyAsync(Key).Result);
            Assert.True(evicted);
        }

        [Fact]
        public void WhenRemoveKeyAsyncNotInCache()
        {
            var backingStoreMock = new Mock<IBackingStore<string, int>>();
            var cache = new UnboundedCache<string, int>(backingStoreMock.Object, new CurrentThreadTaskScheduler());
            var evicted = false;
            cache.KeyEvicted += (s, a) => evicted = a.Key == Key;
            Assert.False(cache.ContainsKeyInCacheAsync(Key).Result);

            var task = cache.RemoveKeyAsync(Key);
            backingStoreMock.Verify(f => f.RemoveKey(Key), Times.Once);
            Assert.False(task.IsFaulted);
            Assert.False(cache.ContainsKeyInCacheAsync(Key).Result);
            Assert.False(cache.ContainsKeyAsync(Key).Result);
            Assert.False(evicted);
        }
    }
}

