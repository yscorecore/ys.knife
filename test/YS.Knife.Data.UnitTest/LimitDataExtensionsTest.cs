using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class LimitDataExtensionsTest
    {
        [TestMethod]
        public void ShouldGetListSourceWhenAsListSource()
        {
            var listSource = Enumerable.Range(1, 100).AsQueryable()
                .ToPagedData(50, 15).ToListSource();
            Assert.AreEqual(true, listSource.ContainsListCollection);
            Assert.AreEqual(15, listSource.GetList().Count);
        }

        [TestMethod]
        public void ShouldGetExpectedLimitData()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToPagedData(50, 15);
            Assert.AreEqual(true, data.HasNext);
            Assert.AreEqual(100, data.TotalCount);
            Assert.AreEqual(15, data.Limit);
            Assert.AreEqual(50, data.Offset);
            Assert.AreEqual(15, data.Items.Count);
            Assert.AreEqual(51, data.Items.First());
            Assert.AreEqual(65, data.Items.Last());

        }

        [TestMethod]
        public void ShouldGetExpectedLimitDataWhenAtStart()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToPagedData(0, 10);
            Assert.AreEqual(true, data.HasNext);
            Assert.AreEqual(100, data.TotalCount);
            Assert.AreEqual(10, data.Limit);
            Assert.AreEqual(0, data.Offset);
            Assert.AreEqual(10, data.Items.Count);
            Assert.AreEqual(1, data.Items.First());
            Assert.AreEqual(10, data.Items.Last());

        }
        [TestMethod]
        public void ShouldGetExpectedLimitDataWhenAtEnd()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToPagedData(90, 10);
            Assert.AreEqual(false, data.HasNext);
            Assert.AreEqual(100, data.TotalCount);
            Assert.AreEqual(10, data.Limit);
            Assert.AreEqual(90, data.Offset);
            Assert.AreEqual(10, data.Items.Count);
            Assert.AreEqual(91, data.Items.First());
            Assert.AreEqual(100, data.Items.Last());

        }

        [TestMethod]
        public void ShouldGetExpectedLimitDataWhenOverlopRange()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToPagedData(95, 15);
            Assert.AreEqual(false, data.HasNext);
            Assert.AreEqual(100, data.TotalCount);
            Assert.AreEqual(15, data.Limit);
            Assert.AreEqual(95, data.Offset);
            Assert.AreEqual(5, data.Items.Count);
            Assert.AreEqual(96, data.Items.First());
            Assert.AreEqual(100, data.Items.Last());
        }

        [TestMethod]
        public void ShouldGetExpectedLimitDataWhenOutRange()
        {
            var data = Enumerable.Range(1, 100).AsQueryable().ToPagedData(120, 15);
            Assert.AreEqual(false, data.HasNext);
            Assert.AreEqual(100, data.TotalCount);
            Assert.AreEqual(15, data.Limit);
            Assert.AreEqual(120, data.Offset);
            Assert.AreEqual(0, data.Items.Count);
        }

    }
}
