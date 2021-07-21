using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.Select
{
    [TestClass]
    public class SelectInfoParserTest
    {
        [TestMethod]
        public void should_return_null_when_parse_empty_string()
        {
            SelectInfo.Parse(null).Should().BeNull();
            SelectInfo.Parse(string.Empty).Should().BeNull();
            SelectInfo.Parse(" \t  ").Should().BeNull();
        }
        [DataTestMethod]
        [DataRow("a","a")]
        [DataRow("abc", "abc")]
        [DataRow("a,b,c", "a,b,c")]
        [DataRow(" a , b , c ", "a,b,c")]
        public void should_parse_simple_select_items(string input, string expected) 
        {
            ParseSelectInfoShouldBe(input, expected);
        }

        [DataTestMethod]
        [DataRow("a()", "a()")]
        [DataRow("a(b)", "a(b)")]
        [DataRow("a(b(c(d())))", "a(b(c(d())))")]
        [DataRow("a ( b ( c ( d ( ) ) ) )", "a(b(c(d())))")]
        [DataRow("a(b,c)", "a(b,c)")]
        [DataRow("a(b(),c)", "a(b(),c)")]
        [DataRow("a(b,c())", "a(b,c())")]
        [DataRow("a(b,c()),d,e(f)", "a(b,c()),d,e(f)")]
        [DataRow("a ( b , c ( ) ) , d , e ( f ) ", "a(b,c()),d,e(f)")]
        public void should_parse_when_has_sub_select_items(string input, string expected)
        {
            ParseSelectInfoShouldBe(input, expected);
        }

        [DataTestMethod]
        [DataRow("a{}", "a")]
        [DataRow("a{limit(1)}", "a{limit(0,1)}")]
        [DataRow("a{limit(1,3)}", "a{limit(1,3)}")]
        [DataRow("a{LIMIT(1,3)}", "a{limit(1,3)}")]
        [DataRow("a{orderby(a)}", "a{orderby(a.asc())}")]
        [DataRow("a{orderby(a.asc)}", "a{orderby(a.asc.asc())}")]
        [DataRow("a{orderby(a.desc())}", "a{orderby(a.desc())}")]
        [DataRow("a{orderby(a.asc(),b.desc())}", "a{orderby(a.asc(),b.desc())}")]
        [DataRow("a{ORDERBY(a.asc(),b.desc())}", "a{orderby(a.asc(),b.desc())}")]
        [DataRow("a{where(a=1)}", "a{where(a == 1)}")]
        [DataRow("a{where((a=1) and (b=2))}", "a{where((a == 1) and (b == 2))}")]
        [DataRow("a{where((a=1) or (b=2))}", "a{where((a == 1) or (b == 2))}")]
        [DataRow("a{limit(3), orderby(abc), where((a=1))}", "a{limit(0,3),orderby(abc.asc()),where(a == 1)}")]
        public void should_parse_when_has_collection_functions(string input, string expected)
        {
            ParseSelectInfoShouldBe(input, expected);
        }


        [ExpectedException(typeof(FilterInfoParseException))]
        [DataTestMethod]
        [DataRow("a{limit()}")]
        [DataRow("a{limit(.1)}")]
        [DataRow("a{limit(1,2,3)}")]
        [DataRow("a{orderby()}")]
        [DataRow("a{orderby(1)}")]
        [DataRow("a{orderby(a=1)}")]
        [DataRow("a{where(a)}")]
        [DataRow("a{where((a)}")]
        [DataRow("a{where((a)}")]
        [DataRow("a{limit(1), limit(2)}")]
        [DataRow("a{orderby(a), orderby(b)}")]
        public void should_throw_exception_when_parse_invalid_collection_functions(string input)
        {
            SelectInfo.Parse(input);
        }
        private void ParseSelectInfoShouldBe(string inputText, string expected)
        {
            SelectInfo.Parse(inputText).ToString().Should().Be(expected);
        }
    }
}
