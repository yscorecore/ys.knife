using FluentAssertions;
using Xunit;

namespace YS.Knife.Data.UnitTest
{
    
    public class FunctionAttributeTest
    {
        [Theory]
        [InlineData(nameof(Class1.SayHello), "FunctionCode1")]
        [InlineData(nameof(Class1.SayHello2), "")]
        [InlineData(nameof(Class1.SayHello3), "YS.Knife.Data.UnitTest.Class1.SayHello3")]
        [InlineData(nameof(Class1.SayHello4), "YS.Knife.Data.UnitTest.Class1.SayHello4")]
        [InlineData(nameof(Class1.SayHello5), "YS.Knife.Data.UnitTest.Class1.SayHello5")]
        [InlineData(nameof(Class1.SayHello6), "Function6")]
        public void ShouldGetExpectedFunctionCode(string methodName, string expected)
        {
            typeof(Class1).GetMethod(methodName).GetFunctionCode().Should().Be(expected);
        }


    }

    public class Class1 : BaseClass
    {
        [Function("FunctionCode1")]
        public void SayHello()
        {

        }
        [Function("")]
        public void SayHello2()
        {

        }
        [Function(null)]
        public void SayHello3()
        {

        }

        public void SayHello4()
        {

        }

        public override void SayHello5()
        {

        }
    }

    public class BaseClass
    {
        [Function("Function5")]
        public virtual void SayHello5()
        {

        }

        [Function("Function6")]
        public virtual void SayHello6()
        {

        }
    }

}
