using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Aop;
namespace YS.Knife.Localization
{
    [TestClass]
    public class StringResourcesTest
    {
        [TestMethod]
        public void ShouldGetMethodValueFromDefaultTemplate()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<I18n>();
            Assert.AreEqual("Hello,World", sr.Hello());
        }

        [TestMethod]
        public void ShouldGetMethodValueFormatIndexValueFromDefaultTemplate()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<I18n>();
            var actual = sr.SayHelloWithIndex("zhangsan", 12);
            Assert.AreEqual("Hello, I'm zhangsan, I'm 012 years old.", sr.SayHelloWithIndex("zhangsan", 12));
        }

        [TestMethod]
        public void ShouldGetMethodValueFormatParamNameValueFromDefaultTemplate()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<I18n>();
            var actual = sr.SayHelloWithName(12, "zhangsan");
            Assert.AreEqual("Hello, I'm zhangsan, I'm 012 years old.", actual);
        }

        [TestMethod]
        public void ShouldGetMethodValueFormatParamNameValueAndIndexValueFromDefaultTemplate()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<I18n>();
            var actual = sr.SayHelloWithNameAndIndex(12, "zhangsan");
            Assert.AreEqual("Hello, I'm zhangsan, I'm 012 years old.", actual);
        }
    }
}
