using System;
using FluentAssertions;
using Xunit;

namespace YS.Knife.Data.Query
{

    public class SelectInfoParserTest
    {
        [Fact]
        public void should_return_null_when_parse_empty_string()
        {
            SelectInfo.Parse(null).Should().BeNull();
            SelectInfo.Parse(string.Empty).Should().BeNull();
            SelectInfo.Parse(" \t  ").Should().BeNull();
        }
        [Theory]
        [InlineData("a", "a")]
        [InlineData("abc", "abc")]
        [InlineData("a,b,c", "a,b,c")]
        [InlineData(" a , b , c ", "a,b,c")]
        public void should_parse_simple_select_items(string input, string expected)
        {
            ParseSelectInfoShouldBe(input, expected);
        }

        [Theory]
        [InlineData("a()", "a()")]
        [InlineData("a(b)", "a(b)")]
        [InlineData("a(b(c(d())))", "a(b(c(d())))")]
        [InlineData("a ( b ( c ( d ( ) ) ) )", "a(b(c(d())))")]
        [InlineData("a(b,c)", "a(b,c)")]
        [InlineData("a(b(),c)", "a(b(),c)")]
        [InlineData("a(b,c())", "a(b,c())")]
        [InlineData("a(b,c()),d,e(f)", "a(b,c()),d,e(f)")]
        [InlineData("a ( b , c ( ) ) , d , e ( f ) ", "a(b,c()),d,e(f)")]
        public void should_parse_when_has_sub_select_items(string input, string expected)
        {
            ParseSelectInfoShouldBe(input, expected);
        }

        [Theory]
        [InlineData("a{}", "a")]
        [InlineData("a{limit(1)}", "a{limit(0,1)}")]
        [InlineData("a{limit(1,3)}", "a{limit(1,3)}")]
        [InlineData("a{LIMIT(1,3)}", "a{limit(1,3)}")]
        [InlineData("a{orderby(a)}", "a{orderby(a.asc())}")]
        [InlineData("a{orderby(a.asc)}", "a{orderby(a.asc.asc())}")]
        [InlineData("a{orderby(a.desc())}", "a{orderby(a.desc())}")]
        [InlineData("a{orderby(a.asc(),b.desc())}", "a{orderby(a.asc(),b.desc())}")]
        [InlineData("a{ORDERBY(a.asc(),b.desc())}", "a{orderby(a.asc(),b.desc())}")]
        [InlineData("a{where(a=1)}", "a{where(a == 1)}")]
        [InlineData("a{where((a=1) and (b=2))}", "a{where((a == 1) and (b == 2))}")]
        [InlineData("a{where((a=1) or (b=2))}", "a{where((a == 1) or (b == 2))}")]
        [InlineData("a{limit(3), orderby(abc), where((a=1))}", "a{limit(0,3),orderby(abc.asc()),where(a == 1)}")]
        public void should_parse_when_has_collection_functions(string input, string expected)
        {
            ParseSelectInfoShouldBe(input, expected);
        }


        [Theory]
        [InlineData("a{limit()}")]
        [InlineData("a{limit(.1)}")]
        [InlineData("a{limit(1,2,3)}")]
        [InlineData("a{orderby()}")]
        [InlineData("a{orderby(1)}")]
        [InlineData("a{orderby(a=1)}")]
        [InlineData("a{where(a)}")]
        [InlineData("a{where((a)}")]
        [InlineData("a{limit(1), limit(2)}")]
        [InlineData("a{orderby(a), orderby(b)}")]
        public void should_throw_exception_when_parse_invalid_collection_functions(string input)
        {
            var action = new Action(() => SelectInfo.Parse(input));
            action.Should().ThrowExactly<FilterInfoParseException>();
        }
        private void ParseSelectInfoShouldBe(string inputText, string expected)
        {
            SelectInfo.Parse(inputText).ToString().Should().Be(expected);
        }
    }
}
