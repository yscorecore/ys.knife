using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace YS.Knife.Localization
{
    [TestClass]
    public class LocalizationTest
    {
        [TestMethod]
        public void ShouldGetKeyWhenNoResourceValue()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<SR>();
            Assert.AreEqual("NoResource", sr.NoResource);
        }

        [TestMethod]
        public void ShouldGetResourceValueWhenConfigResourceValue()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<SR>();
            Assert.AreEqual("Value from resource!", sr.ValueInResource);
        }

        [TestMethod]
        public void ShouldGetEmptyWhenConfigResourceValueAsEmpty()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<SR>();
            Assert.AreEqual(string.Empty, sr.EmptyValueInResource);
        }

        [TestMethod]
        public void ShouldGetKeyWhenFormatNoResourceValue()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<SR>();
            Assert.AreEqual("FormatNoResource", sr.FormatNoResource("bob"));
        }

        [TestMethod]
        public void ShouldGetFormatedValueWhenFormatResourceValue()
        {
            var provider = Utility.BuildProvider();
            var sr = provider.GetRequiredService<SR>();
            Assert.AreEqual("Hello, bob.", sr.FormatValueInResource("bob"));
        }
    }
}
