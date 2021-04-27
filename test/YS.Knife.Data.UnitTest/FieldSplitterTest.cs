using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Translaters;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class FieldSplitterTest
    {
        [DataTestMethod]
        [DataRow("abc")]
        [DataRow(".abc")]
        [DataRow("abc  ")]
        [DataRow("\t abc  \t ")]
        [DataRow(".abc")]
        [DataRow("abc.")]
        [DataRow(".abc.")]
        public void ShouldSplitSingleWord(string input)
        {
            var split = new FieldSplitter();
            var fieldPath = split.Split(input);
            var expected = new FieldPath() { Field = "abc"};
            fieldPath.Should().BeEquivalentTo(expected );
        }
        
        [DataTestMethod]
        [DataRow("abc.bcd.cde.def")]
        [DataRow(".abc.bcd.cde.def.")]
        [DataRow(" abc . bcd . cde . def ")]
        [DataRow(" abc  . \t bcd . \t cde . def ")]
        public void ShouldSplitMultiPaths(string input)
        {
            var split = new FieldSplitter();
            var fieldPath = split.Split(input);
            var expected = new List<FieldPath>
            {
                new() { Field = "abc"},
                new() { Field = "bcd"},
                new() { Field = "cde"},
                new() { Field = "def"}
            };
            fieldPath.Should().BeEquivalentTo(expected );
        }
        [DataTestMethod]
        [DataRow("abc()")]
        [DataRow(" .abc (    ) ")]
        [DataRow(" .abc( \t) ")]
        [DataRow(" .abc( .) ")]
        public void ShouldSplitWithFunction(string input)
        {
            var split = new FieldSplitter();
            var fieldPath = split.Split(input);
            var expected = new List<FieldPath>
            {
                new() { FuncName = "abc", SubPaths = new List<FieldPath>()}
            };
            fieldPath.Should().BeEquivalentTo(expected );
        }
        
        
        [DataTestMethod]
        [DataRow("abc(bcd.cde.def).efg")]
        [DataRow("abc ( bcd . cde . def ) . efg")]
        public void ShouldSplitWithFunctionWithMultiSubPaths(string input)
        {
            var split = new FieldSplitter();
            var fieldPath = split.Split(input);
            var expected = new List<FieldPath>
            {
                new() { FuncName = "abc", SubPaths = new List<FieldPath>()
                {
                    new() { Field = "bcd"},
                    new() { Field = "cde"},
                    new() { Field = "def"}
                }},
                new() { Field = "efg"}
            };
            fieldPath.Should().BeEquivalentTo(expected );
        }
    }
}
