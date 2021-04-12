using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Hosting;

namespace YS.Knife.Aop
{
    [TestClass]
    public class CodeErrorTest : KnifeHost
    {
        [TestMethod]
        public void TestBaseCase()
        {
            var all = this.GetService<IAllErrors>();
            var exception = all.NewError();
            Assert.IsNotNull(exception);
        }

    }

    [CodeExceptions]
    public interface IAllErrors
    {
        [Ce(100, "some error.")]
        Exception NewError();
    }



}
