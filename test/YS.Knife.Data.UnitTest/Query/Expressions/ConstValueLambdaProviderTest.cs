using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace YS.Knife.Data.Query.Expressions
{
    [TestClass]
    public class ConstValueLambdaProviderTest
    {
        [DataTestMethod]
        [DataRow(null, "p => null")]
        [DataRow(1, "p => 1")]
        [DataRow("1", "p => \"1\"")]
        [DataRow(123456L, "p => 123456")]
        [DataRow("abc", "p => \"abc\"")]
        [DataRow(1.1, "p => 1.1")]
        public void ShouldGetExpectedExpressionFromConstValue(object value, string expectedExpression)
        {
            IValueLambdaProvider constLambda = new ConstValueLambdaProvider(typeof(SourceClass), value);
            var lambda = constLambda.GetLambda();
            lambda.ToString().Should().Be(expectedExpression);
        }

        [DataTestMethod]
        [DataRow(null, typeof(bool), "p => False")]
        [DataRow(false, typeof(bool), "p => False")]
        [DataRow(true, typeof(bool), "p => True")]
        [DataRow(null, typeof(bool?), "p => null")]
        [DataRow(false, typeof(bool?), "p => False")]
        [DataRow(true, typeof(bool?), "p => True")]
        [DataRow(true, typeof(string), "p => \"True\"")]
        [DataRow("false", typeof(bool?), "p => False")]
        [DataRow("true", typeof(bool?), "p => True")]
        [DataRow("false", typeof(bool), "p => False")]
        [DataRow("True", typeof(bool), "p => True")]
        [DataRow("FALSE", typeof(bool), "p => False")]
        [DataRow("TRUE", typeof(bool), "p => True")]
        [DataRow(null, typeof(int), "p => 0")]
        [DataRow(1, typeof(int), "p => 1")]
        [DataRow(1, typeof(long), "p => 1")]
        [DataRow(1, typeof(double), "p => 1")]
        [DataRow(1, typeof(decimal), "p => 1")]
        [DataRow(1, typeof(float), "p => 1")]
        [DataRow(1, typeof(uint), "p => 1")]
        [DataRow(1, typeof(string), "p => \"1\"")]
        [DataRow(1.0, typeof(int), "p => 1")]
        [DataRow(1.0, typeof(long), "p => 1")]
        [DataRow(1.0, typeof(double), "p => 1")]
        [DataRow(1.0, typeof(decimal), "p => 1")]
        [DataRow(1.0, typeof(float), "p => 1")]
        [DataRow(1.0, typeof(uint), "p => 1")]
        [DataRow(1.0, typeof(string), "p => \"1\"")]
        [DataRow("1", typeof(int), "p => 1")]
        [DataRow("1", typeof(long), "p => 1")]
        [DataRow("1", typeof(double), "p => 1")]
        [DataRow("1", typeof(decimal), "p => 1")]
        [DataRow("1", typeof(float), "p => 1")]
        [DataRow("1", typeof(uint), "p => 1")]
        [DataRow(null, typeof(int?), "p => null")]
        [DataRow("1", typeof(int?), "p => 1")]
        [DataRow("1", typeof(long?), "p => 1")]
        [DataRow("1", typeof(double?), "p => 1")]
        [DataRow("1", typeof(decimal?), "p => 1")]
        [DataRow("1", typeof(float?), "p => 1")]
        [DataRow("1", typeof(uint?), "p => 1")]
        public void ShouldGetExpectedExpressionWhenValueAsBasicType(object value, Type targetType,
            string expectedExpression)
        {
            IValueLambdaProvider constLambda = new ConstValueLambdaProvider(typeof(SourceClass), value);
            var lambda = constLambda.GetLambda(targetType);
            lambda.ReturnType.Should().Be(targetType);
            lambda.ToString().Should().Be(expectedExpression);
        }
        [DataTestMethod]
        [DataRow(TestEnum.A, typeof(int), "p => 1")]
        [DataRow(TestEnum.A, typeof(double), "p => 1")]
        [DataRow(TestEnum.A, typeof(string), "p => \"A\"")]
        [DataRow(1, typeof(TestEnum), "p => A")]
        [DataRow(1.0, typeof(TestEnum), "p => A")]
        [DataRow("a", typeof(TestEnum), "p => A")]
        [DataRow("A", typeof(TestEnum), "p => A")]
        [DataRow(null, typeof(TestEnum), "p => 0")]
        [DataRow(1, typeof(TestEnum?), "p => A")]
        [DataRow(1.0, typeof(TestEnum?), "p => A")]
        [DataRow("a", typeof(TestEnum?), "p => A")]
        [DataRow("A", typeof(TestEnum?), "p => A")]
        [DataRow(1, typeof(TestEnum?), "p => A")]
        [DataRow(1.0, typeof(TestEnum?), "p => A")]
        [DataRow("a", typeof(TestEnum?), "p => A")]
        [DataRow(null, typeof(TestEnum?), "p => null")]
        public void ShouldGetExpectedExpressionWhenValueAsEnumType(object value, Type targetType,
            string expectedExpression)
        {
            IValueLambdaProvider constLambda = new ConstValueLambdaProvider(typeof(SourceClass), value);
            var lambda = constLambda.GetLambda(targetType);
            lambda.ReturnType.Should().Be(targetType);
            lambda.ToString().Should().Be(expectedExpression);
        }
        [DataTestMethod]
        [DataRow(null, typeof(Guid), "p => 00000000-0000-0000-0000-000000000000")]
        [DataRow("10000000-1000-1000-1000-100000000000", typeof(Guid), "p => 10000000-1000-1000-1000-100000000000")]
        [DataRow(null, typeof(Guid?), "p => null")]
        [DataRow("10000000-1000-1000-1000-100000000000", typeof(Guid?), "p => 10000000-1000-1000-1000-100000000000")]
        public void ShouldGetExpectedExpressionWhenValueAsGuidType(object value, Type targetType,
            string expectedExpression)
        {
            IValueLambdaProvider constLambda = new ConstValueLambdaProvider(typeof(SourceClass), value);
            var lambda = constLambda.GetLambda(targetType);
            lambda.ReturnType.Should().Be(targetType);
            lambda.ToString().Should().Be(expectedExpression);
        }
        [DataTestMethod]
        [DataRow(null, typeof(DateTime), "0001-01-01 00:00:00.000")]
        [DataRow("1970-01-01", typeof(DateTime), "1970-01-01 00:00:00.000")]
        [DataRow("1970-01-01 +00:00", typeof(DateTime), "1970-01-01 00:00:00.000")]
        [DataRow("1970-01-01 00:00:03.001 +00:00", typeof(DateTime), "1970-01-01 00:00:03.001")]
        [DataRow("1970-01-01T00:00:03.001 +00:00", typeof(DateTime), "1970-01-01 00:00:03.001")]
        [DataRow("1970-01-01T00:00:03.001 +08:00", typeof(DateTime), "1970-01-01 00:00:03.001")]
        [DataRow(3001, typeof(DateTime), "1970-01-01 00:00:03.001")]
        [DataRow(3001L, typeof(DateTime), "1970-01-01 00:00:03.001")]
        [DataRow(3000.91, typeof(DateTime), "1970-01-01 00:00:03.001")]
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

        [DataTestMethod]
        [DataRow(null, typeof(DateTimeOffset), -62135596800000L)]
        [DataRow("1970-01-01 +00:00", typeof(DateTimeOffset), 0)]
        [DataRow("1970-01-01 00:00:03.001 +00:00", typeof(DateTimeOffset), 3001)]
        [DataRow("1970-01-01T00:00:03.001 +00:00", typeof(DateTimeOffset), 3001)]
        [DataRow("1970-01-01T00:00:03.001 +08:00", typeof(DateTimeOffset), 3001 - 8 * 60 * 60 * 1000)]
        [DataRow(3001, typeof(DateTimeOffset), 3001)]
        [DataRow(3001L, typeof(DateTimeOffset), 3001)]
        [DataRow(3000.91, typeof(DateTimeOffset), 3001)]
        public void ShouldGetExpectedExpressionWhenValueAsDateTimeOffsetType(object value, Type targetType,
            long timeStamp)
        {
            IValueLambdaProvider constLambda = new ConstValueLambdaProvider(typeof(SourceClass), value);
            var lambda = constLambda.GetLambda(targetType);

            var datetimeOffset = lambda.Compile().DynamicInvoke(new object[] { null });
            datetimeOffset.Should().BeOfType<DateTimeOffset>()
                .Which.ToUnixTimeMilliseconds().Should().Be(timeStamp);
        }


        [DataTestMethod]
        [DataRow("ab", typeof(int))]
        [DataRow("c", typeof(TestEnum))]
        [DataRow("abc", typeof(DateTime))]
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
