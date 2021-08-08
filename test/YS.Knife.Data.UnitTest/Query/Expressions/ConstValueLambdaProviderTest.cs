using System;
using FluentAssertions;
using Xunit;


namespace YS.Knife.Data.Query.Expressions
{

    public class ConstValueLambdaProviderTest
    {
        [Theory]
        [InlineData(null, "p => null")]
        [InlineData(1, "p => 1")]
        [InlineData("1", "p => \"1\"")]
        [InlineData(123456L, "p => 123456")]
        [InlineData("abc", "p => \"abc\"")]
        [InlineData(1.1, "p => 1.1")]
        public void ShouldGetExpectedExpressionFromConstValue(object value, string expectedExpression)
        {
            IValueLambdaProvider constLambda = new ConstValueLambdaProvider(typeof(SourceClass), value);
            var lambda = constLambda.GetLambda();
            lambda.ToString().Should().Be(expectedExpression);
        }

        [Theory]
        [InlineData(null, typeof(bool), "p => False")]
        [InlineData(false, typeof(bool), "p => False")]
        [InlineData(true, typeof(bool), "p => True")]
        [InlineData(null, typeof(bool?), "p => null")]
        [InlineData(false, typeof(bool?), "p => False")]
        [InlineData(true, typeof(bool?), "p => True")]
        [InlineData(true, typeof(string), "p => \"True\"")]
        [InlineData("false", typeof(bool?), "p => False")]
        [InlineData("true", typeof(bool?), "p => True")]
        [InlineData("false", typeof(bool), "p => False")]
        [InlineData("True", typeof(bool), "p => True")]
        [InlineData("FALSE", typeof(bool), "p => False")]
        [InlineData("TRUE", typeof(bool), "p => True")]
        [InlineData(null, typeof(int), "p => 0")]
        [InlineData(1, typeof(int), "p => 1")]
        [InlineData(1, typeof(long), "p => 1")]
        [InlineData(1, typeof(double), "p => 1")]
        [InlineData(1, typeof(decimal), "p => 1")]
        [InlineData(1, typeof(float), "p => 1")]
        [InlineData(1, typeof(uint), "p => 1")]
        [InlineData(1, typeof(string), "p => \"1\"")]
        [InlineData(1.0, typeof(int), "p => 1")]
        [InlineData(1.0, typeof(long), "p => 1")]
        [InlineData(1.0, typeof(double), "p => 1")]
        [InlineData(1.0, typeof(decimal), "p => 1")]
        [InlineData(1.0, typeof(float), "p => 1")]
        [InlineData(1.0, typeof(uint), "p => 1")]
        [InlineData(1.0, typeof(string), "p => \"1\"")]
        [InlineData("1", typeof(int), "p => 1")]
        [InlineData("1", typeof(long), "p => 1")]
        [InlineData("1", typeof(double), "p => 1")]
        [InlineData("1", typeof(decimal), "p => 1")]
        [InlineData("1", typeof(float), "p => 1")]
        [InlineData("1", typeof(uint), "p => 1")]
        [InlineData(null, typeof(int?), "p => null")]
        [InlineData("1", typeof(int?), "p => 1")]
        [InlineData("1", typeof(long?), "p => 1")]
        [InlineData("1", typeof(double?), "p => 1")]
        [InlineData("1", typeof(decimal?), "p => 1")]
        [InlineData("1", typeof(float?), "p => 1")]
        [InlineData("1", typeof(uint?), "p => 1")]
        public void ShouldGetExpectedExpressionWhenValueAsBasicType(object value, Type targetType,
            string expectedExpression)
        {
            IValueLambdaProvider constLambda = new ConstValueLambdaProvider(typeof(SourceClass), value);
            var lambda = constLambda.GetLambda(targetType);
            lambda.ReturnType.Should().Be(targetType);
            lambda.ToString().Should().Be(expectedExpression);
        }
        [Theory]
        [InlineData(TestEnum.A, typeof(int), "p => 1")]
        [InlineData(TestEnum.A, typeof(double), "p => 1")]
        [InlineData(TestEnum.A, typeof(string), "p => \"A\"")]
        [InlineData(1, typeof(TestEnum), "p => A")]
        [InlineData(1.0, typeof(TestEnum), "p => A")]
        [InlineData("a", typeof(TestEnum), "p => A")]
        [InlineData("A", typeof(TestEnum), "p => A")]
        [InlineData(null, typeof(TestEnum), "p => 0")]
        [InlineData(1, typeof(TestEnum?), "p => A")]
        [InlineData(1.0, typeof(TestEnum?), "p => A")]
        [InlineData("a", typeof(TestEnum?), "p => A")]
        [InlineData("A", typeof(TestEnum?), "p => A")]
        [InlineData(1, typeof(TestEnum?), "p => A")]
        [InlineData(1.0, typeof(TestEnum?), "p => A")]
        [InlineData("a", typeof(TestEnum?), "p => A")]
        [InlineData(null, typeof(TestEnum?), "p => null")]
        public void ShouldGetExpectedExpressionWhenValueAsEnumType(object value, Type targetType,
            string expectedExpression)
        {
            IValueLambdaProvider constLambda = new ConstValueLambdaProvider(typeof(SourceClass), value);
            var lambda = constLambda.GetLambda(targetType);
            lambda.ReturnType.Should().Be(targetType);
            lambda.ToString().Should().Be(expectedExpression);
        }
        [Theory]
        [InlineData(null, typeof(Guid), "p => 00000000-0000-0000-0000-000000000000")]
        [InlineData("10000000-1000-1000-1000-100000000000", typeof(Guid), "p => 10000000-1000-1000-1000-100000000000")]
        [InlineData(null, typeof(Guid?), "p => null")]
        [InlineData("10000000-1000-1000-1000-100000000000", typeof(Guid?), "p => 10000000-1000-1000-1000-100000000000")]
        public void ShouldGetExpectedExpressionWhenValueAsGuidType(object value, Type targetType,
            string expectedExpression)
        {
            IValueLambdaProvider constLambda = new ConstValueLambdaProvider(typeof(SourceClass), value);
            var lambda = constLambda.GetLambda(targetType);
            lambda.ReturnType.Should().Be(targetType);
            lambda.ToString().Should().Be(expectedExpression);
        }
        [Theory]
        [InlineData(null, typeof(DateTime), "0001-01-01 00:00:00.000")]
        [InlineData("1970-01-01", typeof(DateTime), "1970-01-01 00:00:00.000")]
        [InlineData("1970-01-01 +00:00", typeof(DateTime), "1970-01-01 00:00:00.000")]
        [InlineData("1970-01-01 00:00:03.001 +00:00", typeof(DateTime), "1970-01-01 00:00:03.001")]
        [InlineData("1970-01-01T00:00:03.001 +00:00", typeof(DateTime), "1970-01-01 00:00:03.001")]
        [InlineData("1970-01-01T00:00:03.001 +08:00", typeof(DateTime), "1970-01-01 00:00:03.001")]
        [InlineData(3001, typeof(DateTime), "1970-01-01 00:00:03.001")]
        [InlineData(3001L, typeof(DateTime), "1970-01-01 00:00:03.001")]
        [InlineData(3000.91, typeof(DateTime), "1970-01-01 00:00:03.001")]
        public void ShouldGetExpectedExpressionWhenValueAsDateTimeType(object value, Type targetType,
            string expectedExpression)
        {
            IValueLambdaProvider constLambda = new ConstValueLambdaProvider(typeof(SourceClass), value);
            var lambda = constLambda.GetLambda(targetType);
            var datetime = lambda.Compile().DynamicInvoke(new object[] { null });
            datetime.Should().BeOfType<DateTime>()
                .Which.ToString("yyyy-MM-dd HH:mm:ss.fff").Should().Be(expectedExpression)
                ;
        }

        [Theory]
        [InlineData(null, typeof(DateTimeOffset), -62135596800000L)]
        [InlineData("1970-01-01 +00:00", typeof(DateTimeOffset), 0)]
        [InlineData("1970-01-01 00:00:03.001 +00:00", typeof(DateTimeOffset), 3001)]
        [InlineData("1970-01-01T00:00:03.001 +00:00", typeof(DateTimeOffset), 3001)]
        [InlineData("1970-01-01T00:00:03.001 +08:00", typeof(DateTimeOffset), 3001 - 8 * 60 * 60 * 1000)]
        [InlineData(3001, typeof(DateTimeOffset), 3001)]
        [InlineData(3001L, typeof(DateTimeOffset), 3001)]
        [InlineData(3000.91, typeof(DateTimeOffset), 3001)]
        public void ShouldGetExpectedExpressionWhenValueAsDateTimeOffsetType(object value, Type targetType,
            long timeStamp)
        {
            IValueLambdaProvider constLambda = new ConstValueLambdaProvider(typeof(SourceClass), value);
            var lambda = constLambda.GetLambda(targetType);

            var datetimeOffset = lambda.Compile().DynamicInvoke(new object[] { null });
            datetimeOffset.Should().BeOfType<DateTimeOffset>()
                .Which.ToUnixTimeMilliseconds().Should().Be(timeStamp);
        }


        [Theory]
        [InlineData("ab", typeof(int))]
        [InlineData("c", typeof(TestEnum))]
        [InlineData("abc", typeof(DateTime))]
        public void ShouldThrowExceptionWhenValueCanNotConvertToTargetType(object value, Type targetType)
        {
            IValueLambdaProvider constLambda = new ConstValueLambdaProvider(typeof(SourceClass), value);
            Action action = () => constLambda.GetLambda(targetType);
            action.Should().Throw<QueryExpressionException>()
                .WithMessage($"convert value * to target type '{targetType.FullName}' error.");
        }

        class SourceClass
        {
            public string Name { get; set; }
        }

        enum TestEnum
        {
            A = 1,
            B = 2
        }
    }
}
