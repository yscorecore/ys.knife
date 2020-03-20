using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace YS.Knife.Options
{
    [TestClass]
    public class OptionsTest
    {
        [TestMethod]
        public void ShouldNotGetNullWhenNotDefineOptionsAttribute()
        {
            var provider = Utility.BuildProvider();
            var options = provider.GetService<IOptions<Custom0Options>>();
            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);
            Assert.AreEqual(default, options.Value.Value);
        }

        [TestMethod]
        public void ShouldGetConfigedValueWhenDefineOptionsAttributeWithEmptyConfigKey()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["Custom1:Value"] = "some_value"
            });
            var options = provider.GetService<IOptions<Custom1Options>>();
            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);
            Assert.AreEqual("some_value", options.Value.Value);
        }

        [TestMethod]
        public void ShouldGetConfigedValueWhenDefineOptionsAttributeConfigKey()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["C2:Value"] = "some_value"
            });
            var options = provider.GetService<IOptions<Custom2Options>>();
            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);
            Assert.AreEqual("some_value", options.Value.Value);
        }

        [TestMethod]
        [ExpectedException(typeof(OptionsValidationException))]
        public void ShouldThrowExceptionWhenDefineDataAnnotationsAndConfigInvalidValue()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["Custom7:Value"] = "not a url value"
            });
            var options = provider.GetService<IOptions<Custom7Options>>();
            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);
        }

        [TestMethod]
        public void ShouldGetExpectedValueWhenDefineDataAnnotationsAndConfigValidValue()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["Custom7:Value"] = "http://localhost"
            });
            var options = provider.GetService<IOptions<Custom7Options>>();
            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);
            Assert.AreEqual("http://localhost", options.Value.Value);
        }

        [TestMethod]
        public void ShouldGetConfigedValueWhenDefineOptionsAttributeConfigKeyIsNested()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["C:B:D:Value"] = "some_value"
            });
            var options = provider.GetService<IOptions<Custom3Options>>();
            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);
            Assert.AreEqual("some_value", options.Value.Value);
        }
        [TestMethod]
        public void ShouldGetConfigedValueWhenDefineOptionsAttributeConfigKeyIsNestedWithDot()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["C:B:D:Value"] = "some_value"
            });
            var options = provider.GetService<IOptions<Custom4Options>>();
            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);
            Assert.AreEqual("some_value", options.Value.Value);
        }
        [TestMethod]
        public void ShouldGetConfigedValueWhenDefineOptionsAttributeConfigKeyIsNestedWithDoubleUnderScore()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["C:B:D:Value"] = "some_value"
            });
            var options = provider.GetService<IOptions<Custom5Options>>();
            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);
            Assert.AreEqual("some_value", options.Value.Value);
        }

        [TestMethod]
        public void ShouldGetConfigedValueWhenDefineOptionsAttributeConfigKeyIsEmptyString()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["Value"] = "some_value"
            });
            var options = provider.GetService<IOptions<Custom6Options>>();
            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);
            Assert.AreEqual("some_value", options.Value.Value);
        }
    }
}
