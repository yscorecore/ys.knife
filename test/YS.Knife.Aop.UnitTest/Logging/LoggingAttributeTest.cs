using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Aop.UnitTest.Logging
{
    [TestClass]
    public class LoggingAttributeTest : YS.Knife.Hosting.KnifeHost
    {
        [TestMethod]
        public void TestMethod1()
        {
            var abc = this.GetService<IAbc>();
            abc.Say("Jim");
        }
    }
    [YS.Knife.ServiceClass(injectType: typeof(IAbc))]
    public class Abc : IAbc
    {

        //public void Say(string name)
        //{
        //    System.Console.WriteLine($"Hello, {name}");
        //}
        public void Say(string name)
        {
            throw new System.NotImplementedException();
        }
    }
    public interface IAbc
    {
        [YS.Knife.Aop.ParameterValidation]
        [YS.Knife.Aop.Logging.LoggingAttribute]
        void Say([Required] string name);
    }
}
