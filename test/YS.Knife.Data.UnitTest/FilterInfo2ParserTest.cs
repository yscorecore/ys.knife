using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class FilterInfo2ParserTest
    {
        private Func<string, FilterInfo2> Parse => (text) => FilterInfo2.Parse(text);
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
        [DataRow("true=true", true, FilterType.Equals, true)]
        [DataRow("1=1", 1, FilterType.Equals, 1)]
        [DataRow(".10=+0.1", 0.1, FilterType.Equals, 0.1)]
        [DataRow("\"\"=\"\"", "", FilterType.Equals, "")]
        [DataRow("\"a\"=\"a\"", "a", FilterType.Equals, "a")]
        [DataRow("\"abc\"=\"abc\"", "abc", FilterType.Equals,"abc")]

        public void ShouldParseConstValue(string text, object expectedLeftValue, FilterType expectedFilterType, object expectedRightValue)
        {
            TestBothConstValueFilter(text, expectedLeftValue, expectedFilterType, expectedRightValue);
        }


        [DataTestMethod]
        [DataRow("User=null", "User", FilterType.Equals, null)]
        [DataRow("User.Name=null", "User.Name", FilterType.Equals, null)]
        [DataRow("a.b.c.d.e.f.g=null", "a.b.c.d.e.f.g", FilterType.Equals, null)]
        [DataRow("User. Name=null", "User.Name", FilterType.Equals, null)]
        [DataRow("User. Name.\tLength=null", "User.Name.Length", FilterType.Equals, null)]
        [DataRow("User . Name\t\t.\t\tLength=null", "User.Name.Length", FilterType.Equals, null)]
        [DataRow(" User . Name\t\t.\t\tLength=null ", "User.Name.Length", FilterType.Equals, null)]
        public void ShouldParseFieldName(string text, string expectedLeftExpression, FilterType expectedFilterType, object expectedRightValue)
        {
            TestRightConstValueFilter(text, expectedLeftExpression, expectedFilterType, expectedRightValue);
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
            TestRightConstValueFilter(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [DataTestMethod]
        [DataRow("a=null", "a", FilterType.Equals, null)]
        [DataRow("a=true", "a", FilterType.Equals, true)]
        [DataRow("a=false", "a", FilterType.Equals, false)]
        [DataRow("a=NULL", "a", FilterType.Equals, null)]
        [DataRow("a=TRUE", "a", FilterType.Equals, true)]
        [DataRow("a=FALSE", "a", FilterType.Equals, false)]
        [DataRow("a=Null", "a", FilterType.Equals, null)]
        [DataRow("a=True", "a", FilterType.Equals, true)]
        [DataRow("a=False", "a", FilterType.Equals, false)]
        public void ShouldParseKeywordValue(string text, string expectedFieldName, FilterType expectedFilterType, object expectedValue)
        {
            TestRightConstValueFilter(text, expectedFieldName, expectedFilterType, expectedValue);
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
            TestRightConstValueFilter(text, expectedFieldName, expectedFilterType, expectedValue);
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
            TestRightConstValueFilter(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [DataTestMethod]
        [DataRow("a==1", "a", FilterType.Equals, 1.0)]
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
            TestRightConstValueFilter(text, expectedFieldName, expectedFilterType, expectedValue);
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
            TestRightConstValueFilter(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [DataTestMethod]
        [DataRow("a.func()=null", "a.func() == null")]
        [DataRow("a.b.c.d.func()=null", "a.b.c.d.func() == null")]
        [DataRow("a.func(1)=null", "a.func(1) == null")]
        [DataRow("a.func(-.1,+.2,3)=null", "a.func(-0.1, 0.2, 3) == null")]
        [DataRow("a.func(\"abc\",+.2,\"bcd\")=null", "a.func(\"abc\", 0.2, \"bcd\") == null")]
        [DataRow("a.func(-.1,+.2,b)=null", "a.func(-0.1, 0.2, b) == null")]
        [DataRow("a.func(-.1,+.2,b,c,d)=null", "a.func(-0.1, 0.2, b, c, d) == null")]
        [DataRow("a.func(-.1,+.2,b=3)=null", "a.func(-0.1, 0.2, b == 3) == null")]
        [DataRow("a.func(-.1,+.2,b,c,d,e=3)=null", "a.func(-0.1, 0.2, b, c, d, e == 3) == null")]
        [DataRow("a.func(b)=null", "a.func(b) == null")]
        [DataRow("a.func(b.c.d)=null", "a.func(b.c.d) == null")]
        [DataRow("a.func(b.c.d!=\"e\")=null", "a.func(b.c.d != \"e\") == null")]
        [DataRow("a.func(b.c,d!=\"e\")=null", "a.func(b.c, d != \"e\") == null")]
        [DataRow("a.func((((b.c!=\"e\")or(c.e in [1 ,2]))))=null", "a.func((b.c != \"e\") or (c.e in [1,2])) == null")]
        [DataRow("a.func(b.c,d.count(e,f=true)>1) = null", "a.func(b.c, d.count(e, f == true) > 1) == null")]
        public void ShouldParseFunction(string expression, string expectedExpression)
        {
            TestFilterExpression(expression, expectedExpression);
        }
        [DataTestMethod]
        [DataRow("(((a>1)))", "a > 1")]
        [DataRow("(a>1) or (b<5) ", "(a > 1) or (b < 5)")]
        [DataRow("((a>1)) or ((b<5)) ", "(a > 1) or (b < 5)")]
        [DataRow("(((a>1)) or ((b<5))) ", "(a > 1) or (b < 5)")]
        [DataRow("(((a>1)) and ((b<5))) ", "(a > 1) and (b < 5)")]
        [DataRow("(a>1) or (b<2) and (c=3) or (d<>4) ", "(a > 1) or ((b < 2) and (c == 3)) or (d != 4)")]
        [DataRow("(a>1) and (b<2) or (c=3) and (d<>4) ", "((a > 1) and (b < 2)) or ((c == 3) and (d != 4))")]
        [DataRow("(a>1) and ((b<2) or (c=3)) and (d<>4) ", "(a > 1) and ((b < 2) or (c == 3)) and (d != 4)")]
        [DataRow("(((a>1) and ((b<2) or (c=3)) and (d<>4) )) ", "(a > 1) and ((b < 2) or (c == 3)) and (d != 4)")]
        public void ShouldParseCombinExpression(string expression, string expectedExpression)
        {
            TestFilterExpression(expression, expectedExpression);
        }
        [DataTestMethod]
        [DataRow("a?.b>1", "a?.b > 1")]
        [DataRow("a?.b>1", "a?.b > 1")]
        [DataRow("a?.b?.c>1", "a?.b?.c > 1")]
        [DataRow("a?.b?.c(e.b.c,d=1)>1", "a?.b?.c(e.b.c, d == 1) > 1")]
        [DataRow("a?.b?.c(e.b.c,d?.e=1)>1", "a?.b?.c(e.b.c, d?.e == 1) > 1")]
        [DataRow("a?.b?.c(e?.b?.c=1)>1", "a?.b?.c(e?.b?.c == 1) > 1")]

        [DataRow("a!.b!=1", "a!.b != 1")]
        [DataRow("a!.b!=1", "a!.b != 1")]
        [DataRow("a!.b!.c!=1", "a!.b!.c != 1")]
        [DataRow("a!.b!.c(e.b.c,d!=1)!=1", "a!.b!.c(e.b.c, d != 1) != 1")]
        [DataRow("a!.b!.c(e.b.c,d!.e!=1)!=1", "a!.b!.c(e.b.c, d!.e != 1) != 1")]
        [DataRow("a!.b!.c(e!.b!.c!=1)!=1", "a!.b!.c(e!.b!.c != 1) != 1")]
        public void ShouldParseOptionalFieldName(string expression, string expectedExpression)
        {
            TestFilterExpression(expression, expectedExpression);
        }


        [ExpectedException(typeof(FilterInfoParseException))]
        [DataTestMethod]
        [DataRow("a=1 a")]
        [DataRow("a=b")]
        [DataRow("a===b")]
        [DataRow("a op b")]
        [DataRow("a=[[]]")]
        [DataRow("a=1234567.8_9")]
        [DataRow("a=1234567.8.9")]
        [DataRow("(a=123")]
        [DataRow("(a=123) and (b=456")]
        [DataRow("(a=123) and b=456")]
        [DataRow("(a=123) and (b=456 a")]
        [DataRow("((a=123) and (b=456)")]
        [DataRow("a.b(cde")]
        [DataRow("a.b(cde=")]
        [DataRow("a.b(cde=0")]
        [DataRow("a.b([])")]
        [DataRow("1ab=1")]
        [DataRow("(1a.b=1)")]
        [DataRow("a.b ( b")]
        [DataRow("a.b(cde,f=3")]
        [DataRow("a=\"\\uklkl\"")]
        [DataRow("a=-")]
        [DataRow("a?.b?.c?()>1")]
        [DataRow("a?.b?.c?>1")]
        [DataRow("a?.b?.c(e?.b.c)>1")]
        [DataRow("a?.b?.c(e.b?.c)>1")]
        [DataRow("a?.b?.c(e.b?.c?,d=1)>1")]
        [DataRow("a.func(b=3,e=3)=null")]
        public void ShouldThrowFilterException(string expression)
        {
            Parse(expression);
        }

        
        private void TestFilterExpression(string expression, string expectedFilterExpression)
        {
            var filter = Parse(expression);
            filter.ToString().Should().Be(expectedFilterExpression);
        }
        private void TestRightConstValueFilter(string expression, string expectedLeftExpression, FilterType expectedFilterType, object expectedRightValue)
        {
            var filter = Parse(expression);
            filter.OpType.Should().Be(OpType.SingleItem);
            filter.Items.Should().BeNull();
            filter.Left.ToString().Should().Be(expectedLeftExpression);
            filter.FilterType.Should().Be(expectedFilterType);
            filter.Right.Should().BeEquivalentTo(new ValueInfo { IsValue = true, Value = expectedRightValue });
        }
        private void TestBothConstValueFilter(string expression, object expectedLeftValue, FilterType expectedFilterType, object expectedRightValue)
        {
            var filter = Parse(expression);
            filter.OpType.Should().Be(OpType.SingleItem);
            filter.Items.Should().BeNull();
            filter.Left.Should().BeEquivalentTo(new ValueInfo { IsValue = true, Value = expectedLeftValue });
            filter.FilterType.Should().Be(expectedFilterType);
            filter.Right.Should().BeEquivalentTo(new ValueInfo { IsValue = true, Value = expectedRightValue });
        }
        private void TestSimpleFilter(string expression, string expectedLeftExpression, FilterType expectedFilterType, string expectedRightExpression)
        {
            var filter = Parse(expression);
            filter.OpType.Should().Be(OpType.SingleItem);
            filter.Items.Should().BeNull();
            filter.Left.ToString().Should().Be(expectedLeftExpression);
            filter.FilterType.Should().Be(expectedFilterType);
            filter.Right.ToString().Should().Be(expectedRightExpression);
        }
    }
}
