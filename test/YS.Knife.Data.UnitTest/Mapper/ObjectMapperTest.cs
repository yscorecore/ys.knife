using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Mapper;
using FluentAssertions;
namespace YS.Knife.Data.UnitTest.Mapper
{
    [TestClass]
    public class ObjectMapperTest
    {
        [TestMethod]
        public void Test()
        {
            var mapper = new ObjectMapper<C1, C2>();
            mapper.Append(p => p.Name, p => p.Nm+p.Nm)
                .Append(p=>p.Age,p=> (DateTimeOffset.Now.Year- p.Birthday.Year));
            var source = new C1() {Nm = "zhangsan", Birthday = DateTimeOffset.Now.AddYears(-10), Address = "xian"};
            var target = source.MapOne(mapper);
            
            target.Should().NotBeNull()
                .And.BeEquivalentTo(new C2() {Name = "zhangsanzhangsan", Age = 10, Address = "xian"});
        }

        public class C1
        {
            public string Address { get; set; }
            public string Nm { get; set; }
            public DateTimeOffset Birthday { get; set; }
        }

        public class C2
        {
            public string Address { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
            
        }
    }
}
