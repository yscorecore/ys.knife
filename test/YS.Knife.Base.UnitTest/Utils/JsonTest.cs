using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Utils
{
    [TestClass]
    public class JsonTest
    {
        [TestMethod]
        public void ShouldWriteMaskSecretProperty()
        {
            var student = new Student { Name = "zs", Secret = "secret", Age = 18, Money = 100 };
            var text = Json.Serialize(student);
            text.Should().Be("{\"name\":\"zs\",\"secret\":\"********\",\"age\":18,\"money\":\"******\"}");
        }
        [TestMethod]
        public void ShouldReadMaskSecretProperty()
        {
            var student = new Student { Name = "zs", Secret = "secret", Age = 18, Money = 100 };
            var text = Json.Serialize(student);
            var newStudent = Json.DeSerialize<Student>(text);
            newStudent.Should().BeEquivalentTo(new Student { Name = "zs", Secret = "********", Age = 18, Money = 0 });
        }

        [TestMethod]
        public void ShouldReadOriginPropertyWhenProvided()
        {
            string jsonText = "{\"Money\":123}";
            var newStudent = Json.DeSerialize<Student>(jsonText);
            newStudent.Should().BeEquivalentTo(new Student { Money = 123 });
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
