using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class FunctionAttributeTest
    {
        [DataTestMethod]
        [DataRow(nameof(Class1.SayHello), "FunctionCode1")]
        [DataRow(nameof(Class1.SayHello2), "")]
        [DataRow(nameof(Class1.SayHello3), "YS.Knife.Data.UnitTest.Class1.SayHello3")]
        [DataRow(nameof(Class1.SayHello4), "YS.Knife.Data.UnitTest.Class1.SayHello4")]
        [DataRow(nameof(Class1.SayHello5), "YS.Knife.Data.UnitTest.Class1.SayHello5")]
        [DataRow(nameof(Class1.SayHello6), "Function6")]
        public void ShouldGetExpectedFunctionCode(string methodName, string expected)
        {
            Assert.AreEqual(expected, typeof(Class1).GetMethod(methodName).GetFunctionCode());
        }


    }

    public class Class1:BaseClass
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
