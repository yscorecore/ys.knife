using System;
using System.Linq;
using FluentAssertions;
using Xunit;
namespace YS.Knife.Data.UnitTest
{

    public class SequentialKeyTest
    {
        [Fact]
        public void ShouldAlwaysCreateSequenceString()
        {
            var t1 = DateTime.UtcNow.Ticks;
            var t2 = DateTimeOffset.UtcNow.Ticks;
            var datas = Enumerable.Range(1, 1000).Select(p => SequentialKey.NewString()).ToList();
            for (int i = 1; i < datas.Count; i++)
            {
                datas[i].CompareTo(datas[i - 1]).Should().BeGreaterThan(0);
            }
        }
    }

}
