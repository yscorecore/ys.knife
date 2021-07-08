using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void ParseSelectInfoShouldBe(string inputText, string expected)
        {
            SelectInfo.Parse(inputText).ToString().Should().Be(expected);
        }
    }
}
