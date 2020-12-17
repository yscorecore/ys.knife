using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class SequentialKeyTest
    {
        [TestMethod]
        public void ShouldAlwaysCreateSequenceString()
        {
            var t1 = DateTime.UtcNow.Ticks;
            var t2 = DateTimeOffset.UtcNow.Ticks;
            var datas = Enumerable.Range(1, 1000).Select(p => SequentialKey.NewString()).ToList();
            for (int i = 1; i < datas.Count; i++)
            {
                Assert.IsTrue(datas[i].CompareTo(datas[i - 1]) > 0);
            }
        }
    }

}
