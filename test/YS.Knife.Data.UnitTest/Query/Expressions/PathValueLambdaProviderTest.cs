using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.Query.Expressions
{
    [TestClass]
    public class PathValueLambdaProviderTest
    {
        [DataTestMethod]
        [DataRow("StrProp", "p => p.StrProp")]
        [DataRow("strprop", "p => p.StrProp")]
        [DataRow("strfield", "p => p.StrField")]
        [DataRow("StrProp.length", "p => p.StrProp.Length")]
        [DataRow("intprop", "p => p.IntProp")]
        [DataRow("TimeProp", "p => p.TimeProp")]
        [DataRow("timeprop.year", "p => p.TimeProp.Year")]
        public void ShouldGetExpectedExpressionFromBaiscMemberPaths(string path, string expectedExpression)
        {
            var valueInfo = ValueInfo.Parse(path);
            var memberVisitor = IMemberVisitor.GetObjectVisitor(typeof(SourceBasicClass));
            var constLambda = new PathValueLambdaProvider<SourceBasicClass>(valueInfo.NavigatePaths, memberVisitor);
            var lambda = constLambda.GetLambda();
            lambda.ToString().Should().Be(expectedExpression);

        }
        [DataTestMethod]
        [DataRow("nullableTimeProp", "p => p.NullableTimeProp")]
        [DataRow("nullableTimeProp.value", "p => p.NullableTimeProp.Value")]
        [DataRow("nullableTimeProp.value.year", "p => p.NullableTimeProp.Value.Year")]
        [DataRow("nullableTimeProp.year", "p => p.NullableTimeProp.Value.Year")]
        [DataRow("nullableTimeProp.month", "p => p.NullableTimeProp.Value.Month")]
        public void ShouldGetExpectedExpressionFromNullableMemberPaths(string path, string expectedExpression)
        {
            var valueInfo = ValueInfo.Parse(path);
            var memberVisitor = IMemberVisitor.GetObjectVisitor(typeof(SourceNullableClass));
            var constLambda = new PathValueLambdaProvider<SourcePropClass>(valueInfo.NavigatePaths, memberVisitor);
            var lambda = constLambda.GetLambda();
            lambda.ToString().Should().Be(expectedExpression);
        }

        class SourceBasicClass
        {
            public string StrProp { get; set; }
            public string StrField = string.Empty;
            public int IntProp { get; set; }
            public DateTimeOffset TimeProp { get; set; }
        }
        class SourceNullableClass
        {
            public DateTimeOffset? NullableTimeProp { get; set; }
        }
        class SourcePropClass
        {
            public string StrProp { get; set; }
            public string StrField = string.Empty;
            public int IntProp { get; set; }
            public DateTimeOffset TimeProp { get; set; }
            public DateTimeOffset? NullableTimeProp { get; set; }
            public SourcePropClass2 Sub { get; set; }
        }
        class SourcePropClass2
        {
            public string StrProp2 { get; set; }
            public SourcePropClass3 Sub2 { get; set; }
        }
        class SourcePropClass3
        {
            public string StrProp3 { get; set; }
            public SourcePropClass3 Sub3 { get; set; }
        }
        class SourcePropClass4
        {
            public string StrProp4 { get; set; }
        }
    }
}
