using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.Filter
{
    [TestClass]
    public class FilterInfoTest
    {
        [TestMethod]
        public void ShouldGetReverseFilterTypeWhenGivenSingleItemAndInvokeNot()
        {
            //var filter = new FilterInfo("Age", Operator.GreaterThan, 1).Not();
            //Assert.AreEqual(CombinSymbol.SingleItem, filter.OpType);
            //Assert.AreEqual(Operator.LessThanOrEqual, filter.FilterType);
            //Assert.AreEqual("Age", filter.FieldName);
            //Assert.AreEqual(1, filter.Value);
        }

        [TestMethod]
        public void ShouldGetReverseFilterTypeWhenGivenAndOpTypeAndInvokeNot()
        {
            //var filter = new FilterInfo("Age", Operator.GreaterThan, 1)
            //    .AndAlso("Name", Operator.StartsWith, "Zhang").Not();
            //Assert.AreEqual(CombinSymbol.OrItems, filter.OpType);
            //Assert.AreEqual(2, filter.Items.Count);
            //Assert.AreEqual("Age", filter.Items.First().FieldName);
            //Assert.AreEqual(Operator.LessThanOrEqual, filter.Items.First().FilterType);
            //Assert.AreEqual(1, filter.Items.First().Value);

            //Assert.AreEqual("Name", filter.Items.Last().FieldName);
            //Assert.AreEqual(Operator.NotStartsWith, filter.Items.Last().FilterType);
            //Assert.AreEqual("Zhang", filter.Items.Last().Value);
        }

        [TestMethod]
        public void ShouldReturnSelfInstanceWhenAndAlsoNull()
        {
            //var filter = new FilterInfo("Age", Operator.GreaterThan, 1);
            //Assert.AreEqual(filter, filter.AndAlso(null));
        }
        [TestMethod]
        public void ShouldReturnSelfInstanceWhenOrElseNull()
        {
            //var filter = new FilterInfo("Age", Operator.GreaterThan, 1);
            //Assert.AreEqual(filter, filter.OrElse(null));
        }
        [TestMethod]
        public void ShouldGetReverseFilterTypeWhenGivenOrOpTypeAndInvokeNot()
        {
            //var filter = new FilterInfo("Age", Operator.GreaterThan, 1)
            //    .OrElse("Name", Operator.StartsWith, "Zhang").Not();
            //Assert.AreEqual(CombinSymbol.AndItems, filter.OpType);
            //Assert.AreEqual(2, filter.Items.Count);
            //Assert.AreEqual("Age", filter.Items.First().FieldName);
            //Assert.AreEqual(Operator.LessThanOrEqual, filter.Items.First().FilterType);
            //Assert.AreEqual(1, filter.Items.First().Value);

            //Assert.AreEqual("Name", filter.Items.Last().FieldName);
            //Assert.AreEqual(Operator.NotStartsWith, filter.Items.Last().FilterType);
            //Assert.AreEqual("Zhang", filter.Items.Last().Value);
        }

        [TestMethod]
        public void ShouldAppendItemsWhenAndAlsoAndGivenOpTypeIsAndItemsAndOtherOpTypeIsSingleItem()
        {
            //var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
            //      .AndAlso("Name", Operator.StartsWith, "Zhang");
            //Assert.AreEqual(CombinSymbol.AndItems, filter.OpType);
            //Assert.AreEqual(2, filter.Items.Count);
            //filter.AndAlso(new FilterInfo("Id", Operator.Equals, "001"));
            //Assert.AreEqual(CombinSymbol.AndItems, filter.OpType);
            //Assert.AreEqual(3, filter.Items.Count);
        }

        [TestMethod]
        public void ShouldAppendItemsWhenAndAlsoAndGivenOpTypeIsAndItemsAndOtherOpTypeIsAndItems()
        {
            //var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
            //      .AndAlso("Name", Operator.StartsWith, "Zhang");
            //Assert.AreEqual(CombinSymbol.AndItems, filter.OpType);
            //Assert.AreEqual(2, filter.Items.Count);
            //var otherAndItems = FilterInfo.CreateAnd(
            //    new FilterInfo("Id", Operator.StartsWith, "0"),
            //    new FilterInfo("Tel", Operator.Contains, "135"));
            //filter.AndAlso(otherAndItems);
            //Assert.AreEqual(CombinSymbol.AndItems, filter.OpType);
            //Assert.AreEqual(4, filter.Items.Count);
        }

        [TestMethod]
        public void ShouldCreateNewAndItemsFilterWhenAndAlsoAndGivenOpTypeIsOrItemsAndOtherTypeIsSignleItem()
        {
            //var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
            //      .OrElse("Name", Operator.StartsWith, "Zhang");
            //Assert.AreEqual(CombinSymbol.OrItems, filter.OpType);
            //Assert.AreEqual(2, filter.Items.Count);
            //var newfilter = filter.AndAlso(new FilterInfo("Id", Operator.Equals, "001"));
            //Assert.AreEqual(CombinSymbol.AndItems, newfilter.OpType);
            //Assert.AreEqual(2, newfilter.Items.Count);


            //Assert.AreEqual(filter, newfilter.Items.First());
            //Assert.AreEqual(CombinSymbol.SingleItem, newfilter.Items.Last().OpType);

        }


        [TestMethod]
        public void ShouldAppendItemsWhenOrElseAndGivenOpTypeIsOrItemsAndOtherOpTypeIsSingleItem()
        {
            //var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
            //      .OrElse("Name", Operator.StartsWith, "Zhang");
            //Assert.AreEqual(CombinSymbol.OrItems, filter.OpType);
            //Assert.AreEqual(2, filter.Items.Count);
            //filter.OrElse(new FilterInfo("Id", Operator.Equals, "001"));
            //Assert.AreEqual(CombinSymbol.OrItems, filter.OpType);
            //Assert.AreEqual(3, filter.Items.Count);
        }

        [TestMethod]
        public void ShouldAppendItemsWhenOrElseAndGivenOpTypeIsOrItemsAndOtherOpTypeIsOrItems()
        {
            //var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
            //      .OrElse("Name", Operator.StartsWith, "Zhang");
            //Assert.AreEqual(CombinSymbol.OrItems, filter.OpType);
            //Assert.AreEqual(2, filter.Items.Count);
            //var otherAndItems = FilterInfo.CreateOr(
            //    new FilterInfo("Id", Operator.StartsWith, "0"),
            //    new FilterInfo("Tel", Operator.Contains, "135"));
            //filter.OrElse(otherAndItems);
            //Assert.AreEqual(CombinSymbol.OrItems, filter.OpType);
            //Assert.AreEqual(4, filter.Items.Count);
        }

        [TestMethod]
        public void ShouldCreateNewOrItemsFilterWhenOrElseAndGivenOpTypeIsAndItemsAndOtherTypeIsSignleItem()
        {
            //var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
            //      .AndAlso("Name", Operator.StartsWith, "Zhang");
            //Assert.AreEqual(CombinSymbol.AndItems, filter.OpType);
            //Assert.AreEqual(2, filter.Items.Count);
            //var newfilter = filter.OrElse(new FilterInfo("Id", Operator.Equals, "001"));
            //Assert.AreEqual(CombinSymbol.OrItems, newfilter.OpType);
            //Assert.AreEqual(2, newfilter.Items.Count);


            //Assert.AreEqual(filter, newfilter.Items.First());
            //Assert.AreEqual(CombinSymbol.SingleItem, newfilter.Items.Last().OpType);

        }

        [TestMethod]
        public void ShouldConvertToStringAndConvertFromString()
        {
            //var filter = FilterInfo.CreateOr(
            //    FilterInfo.CreateAnd(
            //        new FilterInfo("Age", Operator.GreaterThan, 1),
            //        new FilterInfo("Name", Operator.StartsWith, "Zhang")),
            //    FilterInfo.CreateAnd(
            //        new FilterInfo("Id", Operator.StartsWith, "0"),
            //        new FilterInfo("Tel", Operator.Contains, "135")));
            //var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(FilterInfo));
            //Assert.IsTrue(converter.CanConvertTo(typeof(string)));
            //Assert.IsTrue(converter.CanConvertFrom(typeof(string)));
            //var plainText = converter.ConvertTo(filter, typeof(string)) as string;
            //Assert.IsNotNull(plainText);
            //var filter2 = converter.ConvertFrom(plainText) as FilterInfo;
            //Assert.AreEqual(CombinSymbol.OrItems, filter2.OpType);
            //Assert.AreEqual(2, filter2.Items.Count);
        }
        [TestMethod]
        public void ShouldReturnStringExpressionWhenToString()
        {
            //var filter = FilterInfo.CreateOr(
            //        FilterInfo.CreateAnd(
            //            new FilterInfo("Age", Operator.GreaterThan, 1),
            //            new FilterInfo("Name", Operator.StartsWith, "Zhang")),
            //        FilterInfo.CreateAnd(
            //            new FilterInfo("Id", Operator.In, new[] { 1, 3, 4 }),
            //            new FilterInfo("Tel", Operator.Contains, "135")));

            //Assert.AreEqual("((Age > 1) and (Name sw \"Zhang\")) or ((Id in [1,3,4]) and (Tel ct \"135\"))", filter.ToString());
            //Assert.AreEqual("((Age <= 1) or (Name nsw \"Zhang\")) and ((Id nin [1,3,4]) or (Tel nct \"135\"))", filter.Not().ToString());

        }
        [DataTestMethod]
        [DataRow("a", Operator.Equals, null, "a == null")]
        [DataRow("a", Operator.Equals, true, "a == true")]
        [DataRow("a", Operator.Equals, false, "a == false")]
        [DataRow("a", Operator.Equals, 1, "a == 1")]
        [DataRow("a", Operator.Equals, .1, "a == 0.1")]
        [DataRow("a", Operator.Equals, -.1, "a == -0.1")]
        [DataRow("a", Operator.Equals, "zhangsan", @"a == ""zhangsan""")]
        [DataRow("a", Operator.Equals, "\\\"", "a == \"\\\\\\\"\"")]
        public void ShouldReprValueWhenToString(string field, Operator filterType, object value, string expectedString)
        {
            //var filter = FilterInfo.CreateItem(field, Operator.Equals, value);

            //Assert.AreEqual(expectedString, filter.ToString());

        }

    }
}
