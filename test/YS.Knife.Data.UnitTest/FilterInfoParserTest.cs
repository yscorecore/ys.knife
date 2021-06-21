using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class FilterInfoParserTest
    {
        private Func<string, FilterInfo> Parse => (text) => new FilterInfoParser().Parse(text);
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(" \t ")]
        public void ShouldNullWhenTextIsEmpty(string text)
        {
            Parse(text).Should().BeNull();
        }
        [DataTestMethod]
        [DataRow("User=null", "User", FilterType.Equals, null)]
        [DataRow("User.Name=null", "User.Name", FilterType.Equals, null)]
        [DataRow("a.b.c.d.e.f.g=null", "a.b.c.d.e.f.g", FilterType.Equals, null)]
        [DataRow("User. Name=null", "User.Name", FilterType.Equals, null)]
        [DataRow("User. Name.\tLength=null", "User.Name.Length", FilterType.Equals, null)]
        public void ShouldParseFieldName(string text, string expectedFieldName, FilterType expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
        }


        [DataTestMethod]
        [DataRow("a=null", "a", FilterType.Equals, null)]
        [DataRow("a==null", "a", FilterType.Equals, null)]
        [DataRow("a!=null", "a", FilterType.NotEquals, null)]
        [DataRow("a<>null", "a", FilterType.NotEquals, null)]
        [DataRow("a>null", "a", FilterType.GreaterThan, null)]
        [DataRow("a>=null", "a", FilterType.GreaterThanOrEqual, null)]
        [DataRow("a<null", "a", FilterType.LessThan, null)]
        [DataRow("a<=null", "a", FilterType.LessThanOrEqual, null)]
        [DataRow("a bt null", "a", FilterType.Between, null)]
        [DataRow("a nbt null", "a", FilterType.NotBetween, null)]
        [DataRow("a in null", "a", FilterType.In, null)]
        [DataRow("a nin null", "a", FilterType.NotIn, null)]
        [DataRow("a sw null", "a", FilterType.StartsWith, null)]
        [DataRow("a nsw null", "a", FilterType.NotStartsWith, null)]
        [DataRow("a ew null", "a", FilterType.EndsWith, null)]
        [DataRow("a new null", "a", FilterType.NotEndsWith, null)]
        [DataRow("a ct null", "a", FilterType.Contains, null)]
        [DataRow("a nct null", "a", FilterType.NotContains, null)]
        public void ShouldParseFilterType(string text, string expectedFieldName, FilterType expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [DataTestMethod]
        [DataRow("a=null", "a", FilterType.Equals, null)]
        [DataRow("a=undefined", "a", FilterType.Equals, null)]
        [DataRow("a=true", "a", FilterType.Equals, true)]
        [DataRow("a=false", "a", FilterType.Equals, false)]
        [DataRow("a=NULL", "a", FilterType.Equals, null)]
        [DataRow("a=UNDEFINED", "a", FilterType.Equals, null)]
        [DataRow("a=TRUE", "a", FilterType.Equals, true)]
        [DataRow("a=FALSE", "a", FilterType.Equals, false)]
        [DataRow("a=Null", "a", FilterType.Equals, null)]
        [DataRow("a=Undefined", "a", FilterType.Equals, null)]
        [DataRow("a=True", "a", FilterType.Equals, true)]
        [DataRow("a=False", "a", FilterType.Equals, false)]
        public void ShouldParseKeywordValue(string text, string expectedFieldName, FilterType expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [DataTestMethod]
        [DataRow("a=\"\"", "a", FilterType.Equals, "")]
        [DataRow("a=\"b\"", "a", FilterType.Equals, "b")]
        [DataRow("a=\"hello\n,this is a long string.\"", "a", FilterType.Equals, "hello\n,this is a long string.")]
        [DataRow("a=\"hello\\n,this is a long string.\"", "a", FilterType.Equals, "hello\n,this is a long string.")]
        [DataRow("a=\" \"", "a", FilterType.Equals, " ")]
        [DataRow("a=\"\\t\"", "a", FilterType.Equals, "\t")]
        [DataRow("a=\"\\\\\"", "a", FilterType.Equals, "\\")]
        [DataRow("a=\"\\\"\"", "a", FilterType.Equals, "\"")]
        public void ShouldParseStringValue(string text, string expectedFieldName, FilterType expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
        }

        [DataTestMethod]
        [DataRow("a=null", "a", FilterType.Equals, null)]
        [DataRow("(a=null)", "a", FilterType.Equals, null)]
        [DataRow(" a =  null   ", "a", FilterType.Equals, null)]
        [DataRow("( a =  null )  ", "a", FilterType.Equals, null)]
        [DataRow("(( a =   null ))  ", "a", FilterType.Equals, null)]
        [DataRow("((( a =   null )))  ", "a", FilterType.Equals, null)]
        public void ShouldParseSingleItemFilter(string text, string expectedFieldName, FilterType expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [DataTestMethod]
        [DataRow("a=1", "a", FilterType.Equals, 1.0)]
        [DataRow("a=+1", "a", FilterType.Equals, 1.0)]
        [DataRow("a=-1", "a", FilterType.Equals, -1.0)]
        [DataRow("a=.1", "a", FilterType.Equals, 0.1)]
        [DataRow("a=+.1", "a", FilterType.Equals, 0.1)]
        [DataRow("a=-.1", "a", FilterType.Equals, -0.1)]
        [DataRow("a=1.1", "a", FilterType.Equals, 1.1)]
        [DataRow("a=+1.1", "a", FilterType.Equals, 1.1)]
        [DataRow("a=-1.1", "a", FilterType.Equals, -1.1)]
        [DataRow("a=123456789", "a", FilterType.Equals, 123456789)]
        [DataRow("a=123_456_789", "a", FilterType.Equals, 123456789)]
        [DataRow("a=123_456_789.123456", "a", FilterType.Equals, 123456789.123456)]
        [DataRow("a=+123_456_789.123456", "a", FilterType.Equals, 123456789.123456)]
        [DataRow("a=-123_456_789.123456", "a", FilterType.Equals, -123456789.123456)]
        public void ShouldParseNumberValue(string text, string expectedFieldName, FilterType expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [DataTestMethod]
        [DataRow("a=[]", "a", FilterType.Equals, new object[] { })]
        [DataRow("a= [  ] ", "a", FilterType.Equals, new object[] { })]
        [DataRow("a in []", "a", FilterType.In, new object[] { })]
        [DataRow("a in [  \t ]", "a", FilterType.In, new object[] { })]
        [DataRow("a in [-.1]", "a", FilterType.In, new object[] { -.1 })]
        [DataRow("a in [true]", "a", FilterType.In, new object[] { true })]
        [DataRow("a in [\"abc\"]", "a", FilterType.In, new object[] { "abc" })]
        [DataRow("a in [true,null ]", "a", FilterType.In, new object[] { true, null })]
        [DataRow("a in [false , 1_234 , \"abc\"]", "a", FilterType.In, new object[] { false, 1234, "abc" })]
        public void ShouldParseArrayValue(string text, string expectedFieldName, FilterType expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
        }

        private void TestSimpleItem(string text, string expectedFieldName, FilterType expectedFilterType, object expectedValue)
        {
            var filter = Parse(text);
            filter.OpType.Should().Be(OpType.SingleItem);
            filter.Items.Should().BeNull();
            filter.Function.Should().BeNull();
            filter.FieldName.Should().Be(expectedFieldName);
            filter.FilterType.Should().Be(expectedFilterType);
            filter.Value.Should().BeEquivalentTo(expectedValue);
        }
        private void TestComplexItem(string text, string expectedFilterText)
        {
            var filter = Parse(text);
            filter.ToString().Should().Be(expectedFilterText);
        }
    }
}
