using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class FilterInfoTest
    {
        [TestMethod]
        public void ShouldGetReverseFilterTypeWhenGivenSingleItemAndInvokeNot()
        {
            var filter = new FilterInfo("Age", FilterType.GreaterThan, 1).Not();
            Assert.AreEqual(OpType.SingleItem, filter.OpType);
            Assert.AreEqual(FilterType.LessThanOrEqual, filter.FilterType);
            Assert.AreEqual("Age", filter.FieldName);
            Assert.AreEqual(1, filter.Value);
        }

        [TestMethod]
        public void ShouldGetReverseFilterTypeWhenGivenAndOpTypeAndInvokeNot()
        {
            var filter = new FilterInfo("Age", FilterType.GreaterThan, 1)
                .AndAlso("Name", FilterType.StartsWith, "Zhang").Not();
            Assert.AreEqual(OpType.OrItems, filter.OpType);
            Assert.AreEqual(2, filter.Items.Count);
            Assert.AreEqual("Age", filter.Items.First().FieldName);
            Assert.AreEqual(FilterType.LessThanOrEqual, filter.Items.First().FilterType);
            Assert.AreEqual(1, filter.Items.First().Value);

            Assert.AreEqual("Name", filter.Items.Last().FieldName);
            Assert.AreEqual(FilterType.NotStartsWith, filter.Items.Last().FilterType);
            Assert.AreEqual("Zhang", filter.Items.Last().Value);
        }

        [TestMethod]
        public void ShouldReturnSelfInstanceWhenAndAlsoNull()
        {
            var filter = new FilterInfo("Age", FilterType.GreaterThan, 1);
            Assert.AreEqual(filter, filter.AndAlso(null));
        }
        [TestMethod]
        public void ShouldReturnSelfInstanceWhenOrElseNull()
        {
            var filter = new FilterInfo("Age", FilterType.GreaterThan, 1);
            Assert.AreEqual(filter, filter.OrElse(null));
        }
        [TestMethod]
        public void ShouldGetReverseFilterTypeWhenGivenOrOpTypeAndInvokeNot()
        {
            var filter = new FilterInfo("Age", FilterType.GreaterThan, 1)
                .OrElse("Name", FilterType.StartsWith, "Zhang").Not();
            Assert.AreEqual(OpType.AndItems, filter.OpType);
            Assert.AreEqual(2, filter.Items.Count);
            Assert.AreEqual("Age", filter.Items.First().FieldName);
            Assert.AreEqual(FilterType.LessThanOrEqual, filter.Items.First().FilterType);
            Assert.AreEqual(1, filter.Items.First().Value);

            Assert.AreEqual("Name", filter.Items.Last().FieldName);
            Assert.AreEqual(FilterType.NotStartsWith, filter.Items.Last().FilterType);
            Assert.AreEqual("Zhang", filter.Items.Last().Value);
        }

        [TestMethod]
        public void ShouldAppendItemsWhenAndAlsoAndGivenOpTypeIsAndItemsAndOtherOpTypeIsSingleItem()
        {
            var filter = FilterInfo.CreateItem("Age", FilterType.GreaterThan, 1)
                  .AndAlso("Name", FilterType.StartsWith, "Zhang");
            Assert.AreEqual(OpType.AndItems, filter.OpType);
            Assert.AreEqual(2, filter.Items.Count);
            filter.AndAlso(new FilterInfo("Id", FilterType.Equals, "001"));
            Assert.AreEqual(OpType.AndItems, filter.OpType);
            Assert.AreEqual(3, filter.Items.Count);
        }

        [TestMethod]
        public void ShouldAppendItemsWhenAndAlsoAndGivenOpTypeIsAndItemsAndOtherOpTypeIsAndItems()
        {
            var filter = FilterInfo.CreateItem("Age", FilterType.GreaterThan, 1)
                  .AndAlso("Name", FilterType.StartsWith, "Zhang");
            Assert.AreEqual(OpType.AndItems, filter.OpType);
            Assert.AreEqual(2, filter.Items.Count);
            var otherAndItems = FilterInfo.CreateAnd(
                new FilterInfo("Id", FilterType.StartsWith, "0"),
                new FilterInfo("Tel", FilterType.Contains, "135"));
            filter.AndAlso(otherAndItems);
            Assert.AreEqual(OpType.AndItems, filter.OpType);
            Assert.AreEqual(4, filter.Items.Count);
        }

        [TestMethod]
        public void ShouldCreateNewAndItemsFilterWhenAndAlsoAndGivenOpTypeIsOrItemsAndOtherTypeIsSignleItem()
        {
            var filter = FilterInfo.CreateItem("Age", FilterType.GreaterThan, 1)
                  .OrElse("Name", FilterType.StartsWith, "Zhang");
            Assert.AreEqual(OpType.OrItems, filter.OpType);
            Assert.AreEqual(2, filter.Items.Count);
            var newfilter = filter.AndAlso(new FilterInfo("Id", FilterType.Equals, "001"));
            Assert.AreEqual(OpType.AndItems, newfilter.OpType);
            Assert.AreEqual(2, newfilter.Items.Count);


            Assert.AreEqual(filter, newfilter.Items.First());
            Assert.AreEqual(OpType.SingleItem, newfilter.Items.Last().OpType);

        }


        [TestMethod]
        public void ShouldAppendItemsWhenOrElseAndGivenOpTypeIsOrItemsAndOtherOpTypeIsSingleItem()
        {
            var filter = FilterInfo.CreateItem("Age", FilterType.GreaterThan, 1)
                  .OrElse("Name", FilterType.StartsWith, "Zhang");
            Assert.AreEqual(OpType.OrItems, filter.OpType);
            Assert.AreEqual(2, filter.Items.Count);
            filter.OrElse(new FilterInfo("Id", FilterType.Equals, "001"));
            Assert.AreEqual(OpType.OrItems, filter.OpType);
            Assert.AreEqual(3, filter.Items.Count);
        }

        [TestMethod]
        public void ShouldAppendItemsWhenOrElseAndGivenOpTypeIsOrItemsAndOtherOpTypeIsOrItems()
        {
            var filter = FilterInfo.CreateItem("Age", FilterType.GreaterThan, 1)
                  .OrElse("Name", FilterType.StartsWith, "Zhang");
            Assert.AreEqual(OpType.OrItems, filter.OpType);
            Assert.AreEqual(2, filter.Items.Count);
            var otherAndItems = FilterInfo.CreateOr(
                new FilterInfo("Id", FilterType.StartsWith, "0"),
                new FilterInfo("Tel", FilterType.Contains, "135"));
            filter.OrElse(otherAndItems);
            Assert.AreEqual(OpType.OrItems, filter.OpType);
            Assert.AreEqual(4, filter.Items.Count);
        }

        [TestMethod]
        public void ShouldCreateNewOrItemsFilterWhenOrElseAndGivenOpTypeIsAndItemsAndOtherTypeIsSignleItem()
        {
            var filter = FilterInfo.CreateItem("Age", FilterType.GreaterThan, 1)
                  .AndAlso("Name", FilterType.StartsWith, "Zhang");
            Assert.AreEqual(OpType.AndItems, filter.OpType);
            Assert.AreEqual(2, filter.Items.Count);
            var newfilter = filter.OrElse(new FilterInfo("Id", FilterType.Equals, "001"));
            Assert.AreEqual(OpType.OrItems, newfilter.OpType);
            Assert.AreEqual(2, newfilter.Items.Count);


            Assert.AreEqual(filter, newfilter.Items.First());
            Assert.AreEqual(OpType.SingleItem, newfilter.Items.Last().OpType);

        }

    }
}
