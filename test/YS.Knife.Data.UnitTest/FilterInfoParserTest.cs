using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class FilterInfoParserTest
    {
        private Func<string, FilterInfo> Parse => (text) => FilterInfo.Parse(text);
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
        [DataRow("User=null", "User", Operator.Equals, null)]
        [DataRow("User.Name=null", "User.Name", Operator.Equals, null)]
        [DataRow("a.b.c.d.e.f.g=null", "a.b.c.d.e.f.g", Operator.Equals, null)]
        [DataRow("User. Name=null", "User.Name", Operator.Equals, null)]
        [DataRow("User. Name.\tLength=null", "User.Name.Length", Operator.Equals, null)]
        public void ShouldParseFieldName(string text, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
        }

     

        [DataTestMethod]
        [DataRow("a=null", "a", Operator.Equals, null)]
        [DataRow("a==null", "a", Operator.Equals, null)]
        [DataRow("a!=null", "a", Operator.NotEquals, null)]
        [DataRow("a<>null", "a", Operator.NotEquals, null)]
        [DataRow("a>null", "a", Operator.GreaterThan, null)]
        [DataRow("a>=null", "a", Operator.GreaterThanOrEqual, null)]
        [DataRow("a<null", "a", Operator.LessThan, null)]
        [DataRow("a<=null", "a", Operator.LessThanOrEqual, null)]
        [DataRow("a bt null", "a", Operator.Between, null)]
        [DataRow("a nbt null", "a", Operator.NotBetween, null)]
        [DataRow("a in null", "a", Operator.In, null)]
        [DataRow("a nin null", "a", Operator.NotIn, null)]
        [DataRow("a sw null", "a", Operator.StartsWith, null)]
        [DataRow("a nsw null", "a", Operator.NotStartsWith, null)]
        [DataRow("a ew null", "a", Operator.EndsWith, null)]
        [DataRow("a new null", "a", Operator.NotEndsWith, null)]
        [DataRow("a ct null", "a", Operator.Contains, null)]
        [DataRow("a nct null", "a", Operator.NotContains, null)]
        public void ShouldParseFilterType(string text, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [DataTestMethod]
        [DataRow("a=null", "a", Operator.Equals, null)]
        [DataRow("a=true", "a", Operator.Equals, true)]
        [DataRow("a=false", "a", Operator.Equals, false)]
        [DataRow("a=NULL", "a", Operator.Equals, null)]
        [DataRow("a=TRUE", "a", Operator.Equals, true)]
        [DataRow("a=FALSE", "a", Operator.Equals, false)]
        [DataRow("a=Null", "a", Operator.Equals, null)]
        [DataRow("a=True", "a", Operator.Equals, true)]
        [DataRow("a=False", "a", Operator.Equals, false)]
        public void ShouldParseKeywordValue(string text, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [DataTestMethod]
        [DataRow("a=\"\"", "a", Operator.Equals, "")]
        [DataRow("a=\"b\"", "a", Operator.Equals, "b")]
        [DataRow("a=\"hello\n,this is a long string.\"", "a", Operator.Equals, "hello\n,this is a long string.")]
        [DataRow("a=\"hello\\n,this is a long string.\"", "a", Operator.Equals, "hello\n,this is a long string.")]
        [DataRow("a=\" \"", "a", Operator.Equals, " ")]
        [DataRow("a=\"\\t\"", "a", Operator.Equals, "\t")]
        [DataRow("a=\"\\\\\"", "a", Operator.Equals, "\\")]
        [DataRow("a=\"\\\"\"", "a", Operator.Equals, "\"")]
        public void ShouldParseStringValue(string text, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
        }

        [DataTestMethod]
        [DataRow("a=null", "a", Operator.Equals, null)]
        [DataRow("(a=null)", "a", Operator.Equals, null)]
        [DataRow(" a =  null   ", "a", Operator.Equals, null)]
        [DataRow("( a =  null )  ", "a", Operator.Equals, null)]
        [DataRow("(( a =   null ))  ", "a", Operator.Equals, null)]
        [DataRow("((( a =   null )))  ", "a", Operator.Equals, null)]
        public void ShouldParseSingleItemFilter(string text, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [DataTestMethod]
        [DataRow("a==1", "a", Operator.Equals, 1.0)]
        [DataRow("a=+1", "a", Operator.Equals, 1.0)]
        [DataRow("a=-1", "a", Operator.Equals, -1.0)]
        [DataRow("a=.1", "a", Operator.Equals, 0.1)]
        [DataRow("a=+.1", "a", Operator.Equals, 0.1)]
        [DataRow("a=-.1", "a", Operator.Equals, -0.1)]
        [DataRow("a=1.1", "a", Operator.Equals, 1.1)]
        [DataRow("a=+1.1", "a", Operator.Equals, 1.1)]
        [DataRow("a=-1.1", "a", Operator.Equals, -1.1)]
        [DataRow("a=123456789", "a", Operator.Equals, 123456789)]
        [DataRow("a=123_456_789", "a", Operator.Equals, 123456789)]
        [DataRow("a=123_456_789.123456", "a", Operator.Equals, 123456789.123456)]
        [DataRow("a=+123_456_789.123456", "a", Operator.Equals, 123456789.123456)]
        [DataRow("a=-123_456_789.123456", "a", Operator.Equals, -123456789.123456)]
        public void ShouldParseNumberValue(string text, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [DataTestMethod]
        [DataRow("a=[]", "a", Operator.Equals, new object[] { })]
        [DataRow("a= [  ] ", "a", Operator.Equals, new object[] { })]
        [DataRow("a in []", "a", Operator.In, new object[] { })]
        [DataRow("a in [  \t ]", "a", Operator.In, new object[] { })]
        [DataRow("a in [-.1]", "a", Operator.In, new object[] { -.1 })]
        [DataRow("a in [true]", "a", Operator.In, new object[] { true })]
        [DataRow("a in [\"abc\"]", "a", Operator.In, new object[] { "abc" })]
        [DataRow("a in [true,null ]", "a", Operator.In, new object[] { true, null })]
        [DataRow("a in [false , 1_234 , \"abc\"]", "a", Operator.In, new object[] { false, 1234, "abc" })]
        public void ShouldParseArrayValue(string text, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            TestSimpleItem(text, expectedFieldName, expectedFilterType, expectedValue);
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
            TestComplexItem(expression, expectedExpression);
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
            TestComplexItem(expression, expectedExpression);
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
            TestComplexItem(expression, expectedExpression);
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

        private void TestSimpleItem(string expression, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            var filter = Parse(expression);
            filter.OpType.Should().Be(CombinSymbol.SingleItem);
            filter.Items.Should().BeNull();
            filter.Function.Should().BeNull();
            filter.FieldName.Should().Be(expectedFieldName);
            filter.FilterType.Should().Be(expectedFilterType);
            filter.Value.Should().BeEquivalentTo(expectedValue);
        }
        private void TestComplexItem(string expression, string expectedFilterText)
        {
            var filter = Parse(expression);
            filter.ToString().Should().Be(expectedFilterText);
        }
    }

    
}
