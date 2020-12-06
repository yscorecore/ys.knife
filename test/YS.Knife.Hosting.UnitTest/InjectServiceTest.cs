using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using YS.Knife.Hosting;
using YS.Knife.TestData;

namespace YS.Knife
{
    [TestClass]
    public class InjectServiceTest : KnifeHost
    {
        [Inject]
        private ITest test = Mock.Of<ITest>();

        [Inject]
        private ITest2 Prop { get; set; } = Mock.Of<ITest2>();

        [TestMethod]
        public void ShouldGetInjectServiceByCodeWhenDefineInjectAttributeInField()
        {
            Assert.IsNotNull(this.GetService<ITest>());
        }
        [TestMethod]
        public void ShouldGetInjectServiceByCodeWhenDefineInjectAttributeInProperty()
        {
            Assert.IsNotNull(this.GetService<ITest2>());
        }

        public interface ITest { }
        public interface ITest2 { }
    }
}
