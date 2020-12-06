
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class ShouldTest
    {
        [TestMethod]
        public void ShouldBeTrueTest()
        {
            Should.BeTrue(true, 1, "message");
        }
    }
}
