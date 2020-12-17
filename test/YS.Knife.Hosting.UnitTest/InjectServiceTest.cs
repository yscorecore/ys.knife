using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;


namespace YS.Knife.Hosting
{
    [TestClass]
    public class InjectServiceTest : KnifeHost
    {
        [Inject]
        private readonly ITest test = Mock.Of<ITest>();

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
