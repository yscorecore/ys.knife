using System;
using FluentAssertions;
using Xunit;

namespace YS.Knife.Data.Query
{

    public class FilterInfoParserTest
    {
        private Func<string, FilterInfo> Parse => (text) => FilterInfo.Parse(text);
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(" \t ")]
        public void ShouldNullWhenTextIsEmpty(string text)
        {
            Parse(text).Should().BeNull();
        }
        [Theory]
        [InlineData("true=true", true, Operator.Equals, true)]
        [InlineData("1=1", 1, Operator.Equals, 1)]
        [InlineData(".10=+0.1", 0.1, Operator.Equals, 0.1)]
        [InlineData("\"\"=\"\"", "", Operator.Equals, "")]
        [InlineData("\"a\"=\"a\"", "a", Operator.Equals, "a")]
        [InlineData("\"abc\"=\"abc\"", "abc", Operator.Equals, "abc")]

        public void ShouldParseConstValue(string text, object expectedLeftValue, Operator expectedFilterType, object expectedRightValue)
        {
            TestBothConstValueFilter(text, expectedLeftValue, expectedFilterType, expectedRightValue);
        }


        [Theory]
        [InlineData("User=null", "User", Operator.Equals, null)]
        [InlineData("User.Name=null", "User.Name", Operator.Equals, null)]
        [InlineData("a.b.c.d.e.f.g=null", "a.b.c.d.e.f.g", Operator.Equals, null)]
        [InlineData("a  .b   .c   .c . d .e = 1", "a.b.c.c.d.e", Operator.Equals, 1)]
        [InlineData("User. Name=null", "User.Name", Operator.Equals, null)]
        [InlineData("User. Name.\tLength=null", "User.Name.Length", Operator.Equals, null)]
        [InlineData("User . Name\t\t.\t\tLength=null", "User.Name.Length", Operator.Equals, null)]
        [InlineData(" User . Name\t\t.\t\tLength=null ", "User.Name.Length", Operator.Equals, null)]
        public void ShouldParseFieldName(string text, string expectedLeftExpression, Operator expectedFilterType, object expectedRightValue)
        {
            TestRightConstValueFilter(text, expectedLeftExpression, expectedFilterType, expectedRightValue);
        }



        [Theory]
        [InlineData("a=null", "a", Operator.Equals, null)]
        [InlineData("a==null", "a", Operator.Equals, null)]
        [InlineData("a!=null", "a", Operator.NotEquals, null)]
        [InlineData("a<>null", "a", Operator.NotEquals, null)]
        [InlineData("a>null", "a", Operator.GreaterThan, null)]
        [InlineData("a>=null", "a", Operator.GreaterThanOrEqual, null)]
        [InlineData("a<null", "a", Operator.LessThan, null)]
        [InlineData("a<=null", "a", Operator.LessThanOrEqual, null)]
        [InlineData("a bt null", "a", Operator.Between, null)]
        [InlineData("a nbt null", "a", Operator.NotBetween, null)]
        [InlineData("a in null", "a", Operator.In, null)]
        [InlineData("a nin null", "a", Operator.NotIn, null)]
        [InlineData("a sw null", "a", Operator.StartsWith, null)]
        [InlineData("a nsw null", "a", Operator.NotStartsWith, null)]
        [InlineData("a ew null", "a", Operator.EndsWith, null)]
        [InlineData("a new null", "a", Operator.NotEndsWith, null)]
        [InlineData("a ct null", "a", Operator.Contains, null)]
        [InlineData("a nct null", "a", Operator.NotContains, null)]
        public void ShouldParseFilterType(string text, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            TestRightConstValueFilter(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [Theory]
        [InlineData("a=null", "a", Operator.Equals, null)]
        [InlineData("a=true", "a", Operator.Equals, true)]
        [InlineData("a=false", "a", Operator.Equals, false)]
        [InlineData("a=NULL", "a", Operator.Equals, null)]
        [InlineData("a=TRUE", "a", Operator.Equals, true)]
        [InlineData("a=FALSE", "a", Operator.Equals, false)]
        [InlineData("a=Null", "a", Operator.Equals, null)]
        [InlineData("a=True", "a", Operator.Equals, true)]
        [InlineData("a=False", "a", Operator.Equals, false)]
        public void ShouldParseKeywordValue(string text, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            TestRightConstValueFilter(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [Theory]
        [InlineData("a=\"\"", "a", Operator.Equals, "")]
        [InlineData("a=\"b\"", "a", Operator.Equals, "b")]
        [InlineData("a=\"hello\n,this is a long string.\"", "a", Operator.Equals, "hello\n,this is a long string.")]
        [InlineData("a=\"hello\\n,this is a long string.\"", "a", Operator.Equals, "hello\n,this is a long string.")]
        [InlineData("a=\" \"", "a", Operator.Equals, " ")]
        [InlineData("a=\"\\t\"", "a", Operator.Equals, "\t")]
        [InlineData("a=\"\\\\\"", "a", Operator.Equals, "\\")]
        [InlineData("a=\"\\\"\"", "a", Operator.Equals, "\"")]
        public void ShouldParseStringValue(string text, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            TestRightConstValueFilter(text, expectedFieldName, expectedFilterType, expectedValue);
        }

        [Theory]
        [InlineData("a=null", "a", Operator.Equals, null)]
        [InlineData("(a=null)", "a", Operator.Equals, null)]
        [InlineData(" a =  null   ", "a", Operator.Equals, null)]
        [InlineData("( a =  null )  ", "a", Operator.Equals, null)]
        [InlineData("(( a =   null ))  ", "a", Operator.Equals, null)]
        [InlineData("((( a =   null )))  ", "a", Operator.Equals, null)]
        public void ShouldParseSingleItemFilter(string text, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            TestRightConstValueFilter(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [Theory]
        [InlineData("a==1", "a", Operator.Equals, 1.0)]
        [InlineData("a=+1", "a", Operator.Equals, 1.0)]
        [InlineData("a=-1", "a", Operator.Equals, -1.0)]
        [InlineData("a=.1", "a", Operator.Equals, 0.1)]
        [InlineData("a=+.1", "a", Operator.Equals, 0.1)]
        [InlineData("a=-.1", "a", Operator.Equals, -0.1)]
        [InlineData("a=1.1", "a", Operator.Equals, 1.1)]
        [InlineData("a=+1.1", "a", Operator.Equals, 1.1)]
        [InlineData("a=-1.1", "a", Operator.Equals, -1.1)]
        [InlineData("a=123456789", "a", Operator.Equals, 123456789)]
        [InlineData("a=123_456_789", "a", Operator.Equals, 123456789)]
        [InlineData("a=123_456_789.123456", "a", Operator.Equals, 123456789.123456)]
        [InlineData("a=+123_456_789.123456", "a", Operator.Equals, 123456789.123456)]
        [InlineData("a=-123_456_789.123456", "a", Operator.Equals, -123456789.123456)]
        public void ShouldParseNumberValue(string text, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            TestRightConstValueFilter(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [Theory]
        [InlineData("a=[]", "a", Operator.Equals, new object[] { })]
        [InlineData("a= [  ] ", "a", Operator.Equals, new object[] { })]
        [InlineData("a in []", "a", Operator.In, new object[] { })]
        [InlineData("a in [  \t ]", "a", Operator.In, new object[] { })]
        [InlineData("a in [-.1]", "a", Operator.In, new object[] { -.1 })]
        [InlineData("a in [true]", "a", Operator.In, new object[] { true })]
        [InlineData("a in [\"abc\"]", "a", Operator.In, new object[] { "abc" })]
        [InlineData("a in [true,null ]", "a", Operator.In, new object[] { true, null })]
        [InlineData("a in [false , 1_234 , \"abc\"]", "a", Operator.In, new object[] { false, 1234, "abc" })]
        public void ShouldParseArrayValue(string text, string expectedFieldName, Operator expectedFilterType, object expectedValue)
        {
            TestRightConstValueFilter(text, expectedFieldName, expectedFilterType, expectedValue);
        }
        [Theory]
        [InlineData("a.func()=null", "a.func() == null")]
        [InlineData("a.b.c.d.func()=null", "a.b.c.d.func() == null")]
        [InlineData("a.func(1)=null", "a.func(1) == null")]
        [InlineData("a.func(-.1,+.2,3)=null", "a.func(-0.1, 0.2, 3) == null")]
        [InlineData("a.func(\"abc\",+.2,\"bcd\")=null", "a.func(\"abc\", 0.2, \"bcd\") == null")]
        [InlineData("a.func(-.1,+.2,b)=null", "a.func(-0.1, 0.2, b) == null")]
        [InlineData("a.func(-.1,+.2,b,c,d)=null", "a.func(-0.1, 0.2, b, c, d) == null")]
        [InlineData("a.func(b)=null", "a.func(b) == null")]
        [InlineData("a.func(b.c.d)=null", "a.func(b.c.d) == null")]
        [InlineData("a.where(b.c.d!=\"e\")=null", "a.where(b.c.d != \"e\") == null")]
        [InlineData("a.where((((b.c!=\"e\")or(c.e in [1 ,2]))))=null", "a.where((b.c != \"e\") or (c.e in [1,2])) == null")]
        [InlineData("a.where(d.where(f=true).count()>1) = null", "a.where(d.where(f == true).count() > 1) == null")]
        [InlineData("a.where((((b.c!=\"e\")or(c.e in [1 ,2]))))=a.where((((b.c!=\"e\")or(c.e in [1 ,2]))))", "a.where((b.c != \"e\") or (c.e in [1,2])) == a.where((b.c != \"e\") or (c.e in [1,2]))")]
        [InlineData("user.scores.func(score).add2(user.scores.where(class=\"shuxue\").count()).add2(5)=100", "user.scores.func(score).add2(user.scores.where(class == \"shuxue\").count()).add2(5) == 100")]
        [InlineData("a() .b () .c ( ) .c () . d().e=1", "a().b().c().c().d().e == 1")]


        public void ShouldParseFunction(string expression, string expectedExpression)
        {
            TestFilterExpression(expression, expectedExpression);
        }
        [Theory]
        [InlineData("(((a>1)))", "a > 1")]
        [InlineData("(a>1) or (b<5) ", "(a > 1) or (b < 5)")]
        [InlineData("((a>1)) or ((b<5)) ", "(a > 1) or (b < 5)")]
        [InlineData("(((a>1)) or ((b<5))) ", "(a > 1) or (b < 5)")]
        [InlineData("(((a>1)) and ((b<5))) ", "(a > 1) and (b < 5)")]
        [InlineData("(a>1) or (b<2) and (c=3) or (d<>4) ", "(a > 1) or ((b < 2) and (c == 3)) or (d != 4)")]
        [InlineData("(a>1) and (b<2) or (c=3) and (d<>4) ", "((a > 1) and (b < 2)) or ((c == 3) and (d != 4))")]
        [InlineData("(a>1) and ((b<2) or (c=3)) and (d<>4) ", "(a > 1) and ((b < 2) or (c == 3)) and (d != 4)")]
        [InlineData("(((a>1) and ((b<2) or (c=3)) and (d<>4) )) ", "(a > 1) and ((b < 2) or (c == 3)) and (d != 4)")]
        public void ShouldParseCombinExpression(string expression, string expectedExpression)
        {
            TestFilterExpression(expression, expectedExpression);
        }



        [Theory]
        [InlineData("a=1 a")]
        [InlineData("a===b")]
        [InlineData("a op b")]
        [InlineData("a=[[]]")]
        [InlineData("a=1234567.8_9")]
        [InlineData("a=1234567.8.9")]
        [InlineData("(a=123")]
        [InlineData("(a=123) and (b=456")]
        [InlineData("(a=123) and b=456")]
        [InlineData("(a=123) and (b=456 a")]
        [InlineData("((a=123) and (b=456)")]
        [InlineData("a.b(cde")]
        [InlineData("a.b(cde=")]
        [InlineData("a.b(cde=0")]
        [InlineData("a.b([])")]
        [InlineData("1ab=1")]
        [InlineData("(1a.b=1)")]
        [InlineData("a.b ( b")]
        [InlineData("a.b(cde,f=3")]
        [InlineData("a=\"\\uklkl\"")]
        [InlineData("a=-")]
        [InlineData("a?.b?.c?()>1")]
        [InlineData("a?.b?.c?>1")]
        [InlineData("a?.b?.c(e?.b.c)>1")]
        [InlineData("a?.b?.c(e.b?.c)>1")]
        [InlineData("a?.b?.c(e.b?.c?,d=1)>1")]
        [InlineData("a.func(b=3,e=3)=null")]
        public void ShouldThrowFilterException(string expression)
        {
            var action = new Action(() => Parse(expression));
            action.Should().ThrowExactly<FilterInfoParseException>();
        }


        private void TestFilterExpression(string expression, string expectedFilterExpression)
        {
            var filter = Parse(expression);
            filter.ToString().Should().Be(expectedFilterExpression);
        }
        private void TestRightConstValueFilter(string expression, string expectedLeftExpression, Operator expectedFilterType, object expectedRightValue)
        {
            var filter = Parse(expression);
            filter.OpType.Should().Be(CombinSymbol.SingleItem);
            filter.Items.Should().BeNull();
            filter.Left.ToString().Should().Be(expectedLeftExpression);
            filter.Operator.Should().Be(expectedFilterType);
            filter.Right.Should().BeEquivalentTo(new ValueInfo { IsConstant = true, ConstantValue = expectedRightValue });
        }
        private void TestBothConstValueFilter(string expression, object expectedLeftValue, Operator expectedFilterType, object expectedRightValue)
        {
            var filter = Parse(expression);
            filter.OpType.Should().Be(CombinSymbol.SingleItem);
            filter.Items.Should().BeNull();
            filter.Left.Should().BeEquivalentTo(new ValueInfo { IsConstant = true, ConstantValue = expectedLeftValue });
            filter.Operator.Should().Be(expectedFilterType);
            filter.Right.Should().BeEquivalentTo(new ValueInfo { IsConstant = true, ConstantValue = expectedRightValue });
        }
        private void TestSimpleFilter(string expression, string expectedLeftExpression, Operator expectedFilterType, string expectedRightExpression)
        {
            var filter = Parse(expression);
            filter.OpType.Should().Be(CombinSymbol.SingleItem);
            filter.Items.Should().BeNull();
            filter.Left.ToString().Should().Be(expectedLeftExpression);
            filter.Operator.Should().Be(expectedFilterType);
            filter.Right.ToString().Should().Be(expectedRightExpression);
        }
    }
}
