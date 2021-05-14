using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Mapper;
using FluentAssertions;
using System.Linq;

namespace YS.Knife.Data.UnitTest.Mapper
{
    [TestClass]
    public class ObjectMapperTest
    {
        [TestMethod]
        [TestCategory("basic")]
        public void ShouldMapStrPropertyWhenDefineStrMapper()
        {
            var data = new Model
            {
                StrProp = "str"
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendProperty(p => p.StrProp, p => p.StrProp);
            var target = data.MapOne(mapper);
            target.Should().BeEquivalentTo(new DtoModel { StrProp = "str" });
        }
        [TestMethod]
        [TestCategory("basic")]
        public void ShouldGetNullWhenSourceIsNull()
        {
            Model data = null;
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendProperty(p => p.StrProp, p => p.StrProp);
            var target = data.MapOne(mapper);
            target.Should().BeNull();
        }

        [TestMethod]
        [TestCategory("basic")]
        public void ShouldGetEmptyTargetWhenMapperNothing()
        {
            Model data = new Model
            {
                StrProp = "str"
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            var target = data.MapOne(mapper);
            target.Should().BeEquivalentTo(new DtoModel());
        }
        [TestMethod]
        [TestCategory("basic")]
        public void ShouldGetConstValueWhenMapperTargetValueAsConst()
        {
            Model data = new Model
            {
                StrProp = "str"
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendProperty(p => p.StrProp, p => "const");
            var target = data.MapOne(mapper);
            target.Should().BeEquivalentTo(new DtoModel() { StrProp = "const" });
        }
        [TestMethod]
        [TestCategory("basic")]
        public void ShouldGetValueWhenMapperTargetValueToSomeExpression()
        {
            Model data = new Model
            {
                StrProp = "str"
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendProperty(p => p.StrProp, p => "const" + p.StrProp);
            var target = data.MapOne(mapper);
            target.Should().BeEquivalentTo(new DtoModel() { StrProp = "conststr" });
        }

        [TestMethod]
        [TestCategory("basic")]
        public void ShouldGetValueWhenMapperTargetValueToSomeExpression2()
        {
            Model data = new Model
            {
                StrProp = "str"
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendProperty(p => p.StrProp, p => "const" + p.IntProp);
            var target = data.MapOne(mapper);
            target.Should().BeEquivalentTo(new DtoModel() { StrProp = "const0" });
        }
        [TestMethod]
        [TestCategory("basic")]
        public void ShouldGetValueWhenMapNullableToValue()
        {
            Model data = new Model
            {
                NullIntProp = 1
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendProperty(p => p.IntProp, p => p.NullIntProp??0 );
            var target = data.MapOne(mapper);
            target.Should().BeEquivalentTo(new DtoModel() { IntProp = 1 });
        }
        [TestMethod]
        [TestCategory("basic")]
        public void ShouldGetValueWhenMapValueToNullable()
        {
            Model data = new Model
            {
                IntProp = 1
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendProperty(p => p.NullIntProp, p => p.IntProp);
            var target = data.MapOne(mapper);
            target.Should().BeEquivalentTo(new DtoModel() { NullIntProp = 1 });
        }
        [TestMethod]
        [TestCategory("basic")]
        public void ShouldGetPropValueWhenMapToNavigateProp()
        {
            Model data = new Model
            {
                SubModel = new ModelSubModel() { SubStrProp = "str" }
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendProperty(p => p.StrProp, p => p.SubModel != null ? p.SubModel.SubStrProp : null);
            var target = data.MapOne(mapper);
            target.Should().BeEquivalentTo(new DtoModel() { StrProp = "str" });
        }
        [TestMethod]
        public void ShouldGetDeepValueWhenMapComplexObjectAndSourceComplexObjectIsNotNull()
        {
            Model data = new Model
            {
                SubModel = new ModelSubModel() { SubStrProp = "str" }
            };
            var subMapper = new ObjectMapper<ModelSubModel, DtoSubModel>();
            subMapper.AppendProperty(p => p.SubStrProp, p => p.SubStrProp);
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendObject(p => p.SubModel, p => p.SubModel, subMapper);
            var target = data.MapOne(mapper);

            var expected = new DtoModel
            {
                SubModel = new DtoSubModel() { SubStrProp = "str" }
            };

            target.Should().BeEquivalentTo(expected);
        }
        [TestMethod]
        public void ShouldGetNullDeepValueWhenMapComplexObjectAndSourceComplexObjectIsNull()
        {
            Model data = new Model
            {
               
            };

            Test2(p => p == null ? null : new DtoModel());
            var subMapper = new ObjectMapper<ModelSubModel, DtoSubModel>();
            subMapper.AppendProperty(p => p.SubStrProp, p => p.SubStrProp);
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendObject(p => p.SubModel, p => p.SubModel, subMapper);
            var target = data.MapOne(mapper);

            var expected = new DtoModel
            {
                SubModel = null
            };

            target.Should().BeEquivalentTo(expected);
        }


         void Test2(System.Linq.Expressions.Expression<Func<Model,DtoModel>> convert)
        { 
            
        }

        class DtoModel
        {
            public string StrProp { get; set; }
            public int IntProp { get; set; }
            public int? NullIntProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public DateTimeOffset DateTimeOffsetProp { get; set; }
            public DtoSubModel SubModel { get; set; }
        }
        class DtoSubModel
        {
            public string SubStrProp { get; set; }
        }
        class Model
        {
            public string StrProp { get; set; }
            public int IntProp { get; set; }
            public int? NullIntProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public DateTimeOffset DateTimeOffsetProp { get; set; }
            public ModelSubModel SubModel { get; set; }
        }
        public class ModelSubModel
        {
            public string SubStrProp { get; set; }
        }

    }
}
