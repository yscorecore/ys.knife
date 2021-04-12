using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace YS.Knife
{
    [TestClass]
    public class ReflectionExtensionsTest
    {
        [TestMethod]
        public void ShouldGetExpectedDefaultValue()
        {
            Assert.AreEqual(0, typeof(int).DefaultValue());
            Assert.AreEqual(null, typeof(int?).DefaultValue());
            Assert.AreEqual(null, typeof(object).DefaultValue());
        }
    }
}
