using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Hosting;
using Microsoft.Extensions.Options;
namespace YS.Knife.Aws.Kms.UnitTest
{
    [TestClass]
    public class KmsDecryptFunctionTest : KnifeHost
    {
        [TestMethod]
        public void ShouldGetOriginSecretField()
        {
            var testOptions = this.GetService<IOptions<TestOptions>>();
            Assert.AreEqual("zhangsan", testOptions.Value.User);
            Assert.AreEqual("password@123", testOptions.Value.Password);
        }
    }
    [OptionsClass]
    public class TestOptions
    {
        public string User { get; set; }
        public string Password { get; set; }
    }
}
