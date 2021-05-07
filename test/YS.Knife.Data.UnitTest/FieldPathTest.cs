using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Translaters;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class FieldPathTest
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
            var fieldPath = FieldPath.ParsePaths(input);
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
            var fieldPath = FieldPath.ParsePaths(input);
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
            var fieldPath = FieldPath.ParsePaths(input);
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
            var fieldPath = FieldPath.ParsePaths(input);
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
        
        [DataTestMethod]
        [DataRow("abc bcd", 3)]
        [DataRow("abc)", 3)]
        [DataRow("abc((", 4)]
        [DataRow("abc ( bcd cde . def ) . efg", 9)]
        public void ShouldThrowFieldExpressionExceptionWhenParseInvalidPath(string input, int errorIndex)
        {
            Action action = () =>
            {
                 FieldPath.ParsePaths(input);
            };
            action.Should().Throw<FieldExpressionException>()
                .WithMessage($"Invalid field path at index: {errorIndex}.");
        }
        [DataTestMethod]
        [DataRow("abc")]
        [DataRow("abc.bcd")]
        [DataRow("abc.bcd.cde")]
        [DataRow("abc.bcd.cde.def")]
        [DataRow("abc.bcd.cde.func1()")]
        [DataRow("abc.bcd.cde.func1(a)")]
        [DataRow("abc.bcd.cde.func1(a.b)")]
        [DataRow("abc.bcd.cde.func1(a.b.c)")]
        [DataRow("abc.bcd.cde.func1(a)")]
        [DataRow("abc.bcd.cde.func1(a.b)")]
        [DataRow("abc.bcd.cde.func1(a.b.c).def.func2().func3(a)")]
        public void ShouldGetOriginPathWhenJoinPathWithNormalizePath(string normalizePath)
        {
            var paths = FieldPath.ParsePaths(normalizePath);
            FieldPath.JoinPaths(paths).Should().Be(normalizePath);
        }
    }
}
