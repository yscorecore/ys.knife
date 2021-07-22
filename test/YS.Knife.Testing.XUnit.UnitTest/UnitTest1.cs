using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace YS.Knife.Testing.XUnit.UnitTest
{
    [Collection(nameof(Environment))]
    public class UnitTest1
    {
        [Fact]
        public void ShouldRunOnceEnvironment()
        {
            Assert.Equal(1, Environment.Counter);
        }
    }

    [Collection(nameof(Environment))]
    public class UnitTest2
    {
        [Fact]
        public void ShouldRunOnceEnvironment()
        {
            Assert.Equal(1, Environment.Counter);
        }
    }
}
