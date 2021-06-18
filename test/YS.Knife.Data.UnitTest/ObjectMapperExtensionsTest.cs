using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class ObjectMapperExtensionsTest
    {
        private List<Source> datas = new List<Source> {
            new Source {Name = "zhangsan"}, 
            new Source {Name = "zhangsi",Sub = new SubSource(){ Value = "b",Int = 123}}};

        [TestMethod]
        public void ShouldFilterWithNameEquals()
        {
            FilterInfo targetFilter = FilterInfo.CreateItem("TName", FilterType.Equals, "zhangsan");
            var result = FilterSourceData(targetFilter);
            result.Should().BeEquivalentTo(new List<Source> { new Source(){Name = "zhangsan"} });
        }
        [TestMethod]
        public void ShouldFilterWithSubObjectNameEquals()
        {
            FilterInfo targetFilter = FilterInfo.CreateItem("TChildren.Max(TInt)", FilterType.Equals, "b");
            var result = FilterSourceData(targetFilter);
            datas.AsQueryable().Where(p => p.Sub != null && p.Sub.Value == "b");
            result.Should().BeEquivalentTo(new List<Source> { new Source(){Name = "zhangsan"} });
        }

        [TestMethod]
        public void Test()
        {
            
          var method=  typeof(Enumerable).GetMethod("Select", BindingFlags.Public | BindingFlags.Static|BindingFlags.FlattenHierarchy, null, 
              new Type[]{typeof(IEnumerable<>),typeof(Func<,>)}, null);
          var genMethod = method.MakeGenericMethod(typeof(string), typeof(string));
        }

        private List<Source> FilterSourceData(FilterInfo targetFilter)
        {
            var mapper = CreateMapper();
            var exp = mapper.CreateSourceFilterExpression(targetFilter);
            return datas.AsQueryable().Where(exp).ToList();
        }

        private ObjectMapper<Source, Target> CreateMapper()
        {
            var subMapper = new ObjectMapper<SubSource,SubTarget>();
            subMapper.Append(p=>p.TValue,p=>p.Value);
            subMapper.Append(p=>p.TInt,p=>p.Int);
            var mapper = new ObjectMapper<Source, Target>();
            
            mapper.Append(p => p.TName, p => p.Name);
            mapper.Append(p=>p.TSub,p=>p.Sub, subMapper);
            mapper.AppendCollection(p=>p.TChildren,p=>p.Children,subMapper);
            return mapper;
        }

        public class Source
        {
            public string Name { get; set; }
            public SubSource Sub { get; set; }
            
            public List<SubSource> Children { get; set; }
        }
        public class SubSource
        {
            public string Value { get; set; }
            public int Int { get; set; }
        }

        public class Target
        {
            public string TName { get; set; }
            public SubTarget TSub { get; set; }
            
            public List<SubTarget> TChildren { get; set; }
        }
        public class SubTarget
        {
            public string TValue { get; set; }
            public int TInt { get; set; }
        }
    }
}
