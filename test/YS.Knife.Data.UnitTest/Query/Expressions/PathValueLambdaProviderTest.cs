using System;
using FluentAssertions;
using Xunit;

namespace YS.Knife.Data.Query.Expressions
{
    
    public class PathValueLambdaProviderTest
    {
        [Theory]
        [InlineData("StrProp", "p => p.StrProp")]
        [InlineData("strprop", "p => p.StrProp")]
        [InlineData("strfield", "p => p.StrField")]
        [InlineData("StrProp.length", "p => p.StrProp.Length")]
        [InlineData("intprop", "p => p.IntProp")]
        [InlineData("TimeProp", "p => p.TimeProp")]
        [InlineData("timeprop.year", "p => p.TimeProp.Year")]
        public void ShouldGetExpectedExpressionFromBaiscMemberPaths(string path, string expectedExpression)
        {
            var valueInfo = ValueInfo.Parse(path);
            var memberVisitor = IMemberVisitor.GetObjectVisitor(typeof(SourceBasicClass));
            IValueLambdaProvider constLambda = new PathValueLambdaProvider(typeof(SourceBasicClass), valueInfo.NavigatePaths, memberVisitor);
            var lambda = constLambda.GetLambda();
            lambda.ToString().Should().Be(expectedExpression);

        }
        [Theory]
        [InlineData("nullableTimeProp", "p => p.NullableTimeProp")]
        [InlineData("nullableTimeProp.value", "p => p.NullableTimeProp.Value")]
        [InlineData("nullableTimeProp.value.year", "p => p.NullableTimeProp.Value.Year")]
        [InlineData("nullableTimeProp.year", "p => p.NullableTimeProp.Value.Year")]
        [InlineData("nullableTimeProp.month", "p => p.NullableTimeProp.Value.Month")]
        public void ShouldGetExpectedExpressionFromNullableMemberPaths(string path, string expectedExpression)
        {
            var valueInfo = ValueInfo.Parse(path);
            var memberVisitor = IMemberVisitor.GetObjectVisitor(typeof(SourceNullableClass));
            IValueLambdaProvider constLambda = new PathValueLambdaProvider(typeof(SourceNullableClass), valueInfo.NavigatePaths, memberVisitor);
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
