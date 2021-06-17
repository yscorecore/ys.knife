using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class ObjectMapperExtensionsTest
    {
        private List<Source> datas = new List<Source> {new Source {Name = "zhangsan"}, new Source {Name = "zhangsi"}};

        [TestMethod]
        public void Test()
        {
            var mapper = new ObjectMapper<Source, Target>();
            mapper.Append(p => p.Nm, p => p.Name);
            FilterInfo targetFilter = FilterInfo.CreateItem("Nm", FilterType.Equals, "zhangsan");
            var exp = mapper.CreateFilterExpression(targetFilter);
            var result= datas.AsQueryable().Where(exp).ToList();
            result.Should().BeEquivalentTo(new List<Source> { new Source(){Name = "zhangsan"} });
        }

        public class Source
        {
            public string Name { get; set; }
        }

        public class Target
        {
            public string Nm { get; set; }
        }
    }
}
