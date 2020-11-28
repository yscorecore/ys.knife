using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class LimitDataExtensionsTest
    {
        [TestMethod]
        public void ShouldGetExpectedLimitData()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToLimitData(50, 15);
            Assert.AreEqual(true, data.HasNext);
            Assert.AreEqual(100, data.TotalCount);
            Assert.AreEqual(15, data.Limit);
            Assert.AreEqual(50, data.Offset);
            Assert.AreEqual(15, data.ListData.Count);
            Assert.AreEqual(51, data.ListData.First());
            Assert.AreEqual(65, data.ListData.Last());

        }

        [TestMethod]
        public void ShouldGetExpectedLimitDataWhenAtStart()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToLimitData(0, 10);
            Assert.AreEqual(true, data.HasNext);
            Assert.AreEqual(100, data.TotalCount);
            Assert.AreEqual(10, data.Limit);
            Assert.AreEqual(0, data.Offset);
            Assert.AreEqual(10, data.ListData.Count);
            Assert.AreEqual(1, data.ListData.First());
            Assert.AreEqual(10, data.ListData.Last());

        }
        [TestMethod]
        public void ShouldGetExpectedLimitDataWhenAtEnd()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToLimitData(90, 10);
            Assert.AreEqual(false, data.HasNext);
            Assert.AreEqual(100, data.TotalCount);
            Assert.AreEqual(10, data.Limit);
            Assert.AreEqual(90, data.Offset);
            Assert.AreEqual(10, data.ListData.Count);
            Assert.AreEqual(91, data.ListData.First());
            Assert.AreEqual(100, data.ListData.Last());

        }

        [TestMethod]
        public void ShouldGetExpectedLimitDataWhenOverlopRange()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToLimitData(95, 15);
            Assert.AreEqual(false, data.HasNext);
            Assert.AreEqual(100, data.TotalCount);
            Assert.AreEqual(15, data.Limit);
            Assert.AreEqual(95, data.Offset);
            Assert.AreEqual(5, data.ListData.Count);
            Assert.AreEqual(96, data.ListData.First());
            Assert.AreEqual(100, data.ListData.Last());
        }

        [TestMethod]
        public void ShouldGetExpectedLimitDataWhenOutRange()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToLimitData(120, 15);
            Assert.AreEqual(false, data.HasNext);
            Assert.AreEqual(100, data.TotalCount);
            Assert.AreEqual(15, data.Limit);
            Assert.AreEqual(120, data.Offset);
            Assert.AreEqual(0, data.ListData.Count);
        }

    }
}
