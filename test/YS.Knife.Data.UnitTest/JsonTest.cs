using System.Text.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.UnitTest
{

    [TestClass]
    public class JsonTest
    {
        [TestMethod]
        public void ShouldMaskSecretProperty()
        {
            var student = new Student { Name = "zs", Secret = "secret", Age = 18, Money = 100 };
            var text = Json.Serialize(student);
            Assert.AreEqual("{\"name\":\"zs\",\"secret\":\"********\",\"age\":18,\"money\":\"******\"}", text);
        }

        public class Student
        {
            public string Name { get; set; }
            [JsonMask(8)]
            public string Secret { get; set; }
            public int Age { get; set; }
            [JsonMask]
            public double Money { get; set; }
        }
    }

}
