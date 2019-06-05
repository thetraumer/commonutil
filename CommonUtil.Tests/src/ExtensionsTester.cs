using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonUtil;
using Xunit;

namespace CommonUtil.Tests {

    public class ExtensionsTester {

        [Fact]
        public void TestForeach() {
            List<int> list = new List<int>();
            for (int i = 3; i < 10; ++i)
                list.Add(i);
            ICollection<int> collection = (ICollection<int>) list;
            int sum = 0;
            collection.ForEach(i => sum += i);
            Assert.Equal(42, sum);
        }

        [Fact]
        public void TestExists() {
            List<int> list = new List<int>();
            for (int i = 3; i < 10; ++i)
                list.Add(i);
            ICollection<int> collection = (ICollection<int>) list;
            Assert.True(collection.Exists(i => i == 4));
            Assert.False(collection.Exists(i => i == 2));
        }
    }
}
