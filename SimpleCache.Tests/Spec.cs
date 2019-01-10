using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SimpleCache.Tests
{
    public class Spec<TSubject>
    {
        protected TSubject Subject { get; set; }

        protected Action Setup { get; set; }

        protected Action Because { get; set; }

        protected Action It { get; set; }

        [Fact]
        public void Test()
        {
            Setup?.Invoke();
            Because?.Invoke();
            It?.Invoke();
        }

    }
}
