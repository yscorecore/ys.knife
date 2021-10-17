using System.Linq;
using FluentAssertions;
using Xunit;

namespace YS.Knife.Data.Query
{

    public class FilterInfoTest
    {
        [Fact]
        public void ShouldGetReverseFilterTypeWhenGivenSingleItemAndInvokeNot()
        {
            var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1).Not();
            filter.OpType.Should().Be(CombinSymbol.SingleItem);
            filter.Operator.Should().Be(Operator.LessThanOrEqual);
            filter.Left.Should().BeEquivalentTo(ValueInfo.Parse("Age"));
            filter.Right.Should().BeEquivalentTo(ValueInfo.FromConstantValue(1));
        }

        [Fact]
        public void ShouldGetReverseFilterTypeWhenGivenAndOpTypeAndInvokeNot()
        {
            var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
                .AndAlso("Name", Operator.StartsWith, "Zhang").Not();
            filter.OpType.Should().Be(CombinSymbol.OrItems);
            filter.Items.Count.Should().Be(2);
            filter.Items.First().Left.Should().BeEquivalentTo(ValueInfo.Parse("Age"));
            filter.Items.First().Operator.Should().Be(Operator.LessThanOrEqual);
            filter.Items.First().Right.Should().BeEquivalentTo(ValueInfo.FromConstantValue(1));

            filter.Items.Last().Left.Should().BeEquivalentTo(ValueInfo.Parse("Name"));
            filter.Items.Last().Operator.Should().Be(Operator.NotStartsWith);
            filter.Items.Last().Right.Should().BeEquivalentTo(ValueInfo.FromConstantValue("Zhang"));
        }

        [Fact]
        public void ShouldReturnSelfInstanceWhenAndAlsoNull()
        {
            var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1);
            filter.AndAlso(null).Should().Be(filter);
        }
        [Fact]
        public void ShouldReturnSelfInstanceWhenOrElseNull()
        {
            var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1);
            filter.OrElse(null).Should().Be(filter);
        }
        [Fact]
        public void ShouldGetReverseFilterTypeWhenGivenOrOpTypeAndInvokeNot()
        {
            var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
                .OrElse("Name", Operator.StartsWith, "Zhang").Not();
            filter.OpType.Should().Be(CombinSymbol.AndItems);
            filter.Items.Count.Should().Be(2);
            filter.Items.First().Left.Should().BeEquivalentTo(ValueInfo.Parse("Age"));
            filter.Items.First().Operator.Should().Be(Operator.LessThanOrEqual);
            filter.Items.First().Right.Should().BeEquivalentTo(ValueInfo.FromConstantValue(1));

            filter.Items.Last().Left.Should().BeEquivalentTo(ValueInfo.Parse("Name"));
            filter.Items.Last().Operator.Should().Be(Operator.NotStartsWith);
            filter.Items.Last().Right.Should().BeEquivalentTo(ValueInfo.FromConstantValue("Zhang"));
        }

        [Fact]
        public void ShouldAppendItemsWhenAndAlsoAndGivenOpTypeIsAndItemsAndOtherOpTypeIsSingleItem()
        {
            var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
                  .AndAlso("Name", Operator.StartsWith, "Zhang");
            filter.OpType.Should().Be(CombinSymbol.AndItems);
            filter.Items.Count.Should().Be(2);
            filter.AndAlso("Id", Operator.Equals, "001");
            filter.OpType.Should().Be(CombinSymbol.AndItems);
            filter.Items.Count.Should().Be(3);
        }

        [Fact]
        public void ShouldAppendItemsWhenAndAlsoAndGivenOpTypeIsAndItemsAndOtherOpTypeIsAndItems()
        {
            var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
                  .AndAlso("Name", Operator.StartsWith, "Zhang");
            filter.OpType.Should().Be(CombinSymbol.AndItems);
            filter.Items.Count.Should().Be(2);
            var otherAndItems = FilterInfo.CreateAnd(
                FilterInfo.CreateItem("Id", Operator.StartsWith, "0"),
                FilterInfo.CreateItem("Tel", Operator.Contains, "135"));
            filter.AndAlso(otherAndItems);
            filter.OpType.Should().Be(CombinSymbol.AndItems);
            filter.Items.Count.Should().Be(4);
        }

        [Fact]
        public void ShouldCreateNewAndItemsFilterWhenAndAlsoAndGivenOpTypeIsOrItemsAndOtherTypeIsSignleItem()
        {
            var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
                  .OrElse("Name", Operator.StartsWith, "Zhang");
            filter.OpType.Should().Be(CombinSymbol.OrItems);
            filter.Items.Count.Should().Be(2);
            var newfilter = filter.AndAlso("Id", Operator.Equals, "001");
            newfilter.OpType.Should().Be(CombinSymbol.AndItems);
            newfilter.Items.Count.Should().Be(2);


            newfilter.Items.First().Should().Be(filter);
            newfilter.Items.Last().OpType.Should().Be(CombinSymbol.SingleItem);

        }


        [Fact]
        public void ShouldAppendItemsWhenOrElseAndGivenOpTypeIsOrItemsAndOtherOpTypeIsSingleItem()
        {
            var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
                  .OrElse("Name", Operator.StartsWith, "Zhang");
            filter.OpType.Should().Be(CombinSymbol.OrItems);
            filter.Items.Count.Should().Be(2);
            filter.OrElse(FilterInfo.CreateItem("Id", Operator.Equals, "001"));
            filter.OpType.Should().Be(CombinSymbol.OrItems);
            filter.Items.Count.Should().Be(3);
        }

        [Fact]
        public void ShouldAppendItemsWhenOrElseAndGivenOpTypeIsOrItemsAndOtherOpTypeIsOrItems()
        {
            var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
                  .OrElse("Name", Operator.StartsWith, "Zhang");
            filter.OpType.Should().Be(CombinSymbol.OrItems);
            filter.Items.Count.Should().Be(2);
            var otherAndItems = FilterInfo.CreateOr(
                FilterInfo.CreateItem("Id", Operator.StartsWith, "0"),
                FilterInfo.CreateItem("Tel", Operator.Contains, "135"));
            filter.OrElse(otherAndItems);
            filter.OpType.Should().Be(CombinSymbol.OrItems);
            filter.Items.Count.Should().Be(4);
        }

        [Fact]
        public void ShouldCreateNewOrItemsFilterWhenOrElseAndGivenOpTypeIsAndItemsAndOtherTypeIsSignleItem()
        {
            var filter = FilterInfo.CreateItem("Age", Operator.GreaterThan, 1)
                  .AndAlso("Name", Operator.StartsWith, "Zhang");
            filter.OpType.Should().Be(CombinSymbol.AndItems);
            filter.Items.Count.Should().Be(2);
            var newfilter = filter.OrElse("Id", Operator.Equals, "001");
            newfilter.OpType.Should().Be(CombinSymbol.OrItems);
            newfilter.Items.Count.Should().Be(2);


            newfilter.Items.First().Should().Be(filter);
            newfilter.Items.Last().OpType.Should().Be(CombinSymbol.SingleItem);

        }

        [Fact]
        public void ShouldConvertToStringAndConvertFromString()
        {
            var filter = FilterInfo.CreateOr(
                FilterInfo.CreateAnd(
                    FilterInfo.CreateItem("Age", Operator.GreaterThan, 1),
                    FilterInfo.CreateItem("Name", Operator.StartsWith, "Zhang")),
                FilterInfo.CreateAnd(
                    FilterInfo.CreateItem("Id", Operator.StartsWith, "0"),
                   FilterInfo.CreateItem("Tel", Operator.Contains, "135")));
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(FilterInfo));
            converter.CanConvertTo(typeof(string)).Should().BeTrue();
            converter.CanConvertFrom(typeof(string)).Should().BeTrue();
            var plainText = converter.ConvertTo(filter, typeof(string)) as string;
            plainText.Should().NotBeNull();
            var filter2 = converter.ConvertFrom(plainText) as FilterInfo;
            filter2.OpType.Should().Be(CombinSymbol.OrItems);
            filter2.Items.Count.Should().Be(2);
        }
        [Fact]
        public void ShouldReturnStringExpressionWhenToString()
        {
            var filter = FilterInfo.CreateOr(
                    FilterInfo.CreateAnd(
                        FilterInfo.CreateItem("Age", Operator.GreaterThan, 1),
                        FilterInfo.CreateItem("Name", Operator.StartsWith, "Zhang")),
                    FilterInfo.CreateAnd(
                        FilterInfo.CreateItem("Id", Operator.In, new[] { 1, 3, 4 }),
                        FilterInfo.CreateItem("Tel", Operator.Contains, "135")));

            filter.ToString().Should().Be("((Age > 1) and (Name sw \"Zhang\")) or ((Id in [1,3,4]) and (Tel ct \"135\"))");
            filter.Not().ToString().Should().Be("((Age <= 1) or (Name nsw \"Zhang\")) and ((Id nin [1,3,4]) or (Tel nct \"135\"))");

        }
        [Theory]
        [InlineData("a", Operator.Equals, null, "a == null")]
        [InlineData("a", Operator.Equals, true, "a == true")]
        [InlineData("a", Operator.Equals, false, "a == false")]
        [InlineData("a", Operator.Equals, 1, "a == 1")]
        [InlineData("a", Operator.Equals, .1, "a == 0.1")]
        [InlineData("a", Operator.Equals, -.1, "a == -0.1")]
        [InlineData("a", Operator.Equals, "zhangsan", @"a == ""zhangsan""")]
        [InlineData("a", Operator.Equals, "\\\"", "a == \"\\\\\\\"\"")]
        public void ShouldReprValueWhenToString(string field, Operator filterType, object value, string expectedString)
        {
            var filter = FilterInfo.CreateItem(field, filterType, value);

            filter.ToString().Should().Be(expectedString);

        }

    }
}
