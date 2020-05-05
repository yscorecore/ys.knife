using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Aop.UnitTest.Logging
{
    [TestClass]
    public class LoggingAttributeTest:YS.Knife.Hosting.KnifeHost
    {
        [TestMethod]
        public void TestMethod1()
        {
            var abc = this.GetService<Abc>();
            abc.Say("Jim");
        }
    }
    [YS.Knife.ServiceClass]
    public class Abc
    {
        [YS.Knife.Aop.Logging.LoggingAttribute]
        public void Say(string name)
        {
            System.Console.WriteLine($"Hello, {name}");
        }
    }
}