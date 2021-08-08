using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data.UnitTest.Mapper
{
    
    public class ObjectMapperTest
    {
        [Fact]
        public void ShouldMapStrPropertyWhenDefineStrMapper()
        {
            var data = new Model
            {
                StrProp = "str"
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.Append(p => p.StrProp, p => p.StrProp);
            var target = data.Map(mapper);
            target.Should().BeEquivalentTo(new DtoModel { StrProp = "str" });
        }


        [Fact]
        public void ShouldGetNullWhenSourceIsNull()
        {
            Model data = null;
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.Append(p => p.StrProp, p => p.StrProp);
            var target = data.Map(mapper);
            target.Should().BeNull();
        }

        [Fact]
        public void ShouldGetEmptyTargetWhenMapperNothing()
        {
            Model data = new Model
            {
                StrProp = "str"
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            var target = data.Map(mapper);
            target.Should().BeEquivalentTo(new DtoModel());
        }
        [Fact]
        public void ShouldGetConstValueWhenMapperTargetValueAsConst()
        {
            Model data = new Model
            {
                StrProp = "str"
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.Append(p => p.StrProp, p => "const");
            var target = data.Map(mapper);
            target.Should().BeEquivalentTo(new DtoModel() { StrProp = "const" });
        }
        [Fact]
        public void ShouldGetValueWhenMapperTargetValueToSomeExpression()
        {
            Model data = new Model
            {
                StrProp = "str"
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.Append(p => p.StrProp, p => "const" + p.StrProp);
            var target = data.Map(mapper);
            target.Should().BeEquivalentTo(new DtoModel() { StrProp = "conststr" });
        }

        [Fact]
        public void ShouldGetValueWhenMapperTargetValueToSomeExpression2()
        {
            Model data = new Model
            {
                StrProp = "str"
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.Append(p => p.StrProp, p => "const" + p.IntProp);
            var target = data.Map(mapper);
            target.Should().BeEquivalentTo(new DtoModel() { StrProp = "const0" });
        }
        [Fact]
        public void ShouldGetValueWhenMapNullableToValue()
        {
            Model data = new Model
            {
                NullIntProp = 1
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.Append(p => p.IntProp, p => p.NullIntProp ?? 0);
            var target = data.Map(mapper);
            target.Should().BeEquivalentTo(new DtoModel() { IntProp = 1 });
        }
        [Fact]
        public void ShouldGetValueWhenMapValueToNullable()
        {
            Model data = new Model
            {
                IntProp = 1
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.Append(p => p.NullIntProp, p => (int?)p.IntProp);
            var target = data.Map(mapper);
            target.Should().BeEquivalentTo(new DtoModel() { NullIntProp = 1 });
        }
        [Fact]
        public void ShouldGetPropValueWhenMapToNavigateProp()
        {
            Model data = new Model
            {
                SubModel = new ModelSubModel() { SubStrProp = "str" }
            };
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.Append(p => p.StrProp, p => p.SubModel != null ? p.SubModel.SubStrProp : null);
            var target = data.Map(mapper);
            target.Should().BeEquivalentTo(new DtoModel() { StrProp = "str" });
        }
        [Fact]
        public void ShouldGetDeepValueWhenMapComplexObjectAndSourceComplexObjectIsNotNull()
        {
            Model data = new Model
            {
                SubModel = new ModelSubModel() { SubStrProp = "str" }
            };
            var subMapper = new ObjectMapper<ModelSubModel, DtoSubModel>();
            subMapper.Append(p => p.SubStrProp, p => p.SubStrProp);
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.Append(p => p.SubModel, p => p.SubModel, subMapper);
            var target = data.Map(mapper);

            var expected = new DtoModel
            {
                SubModel = new DtoSubModel() { SubStrProp = "str" }
            };

            target.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public void ShouldGetNullDeepValueWhenMapComplexObjectAndSourceComplexObjectIsNull()
        {
            Model data = new Model
            {

            };

            var subMapper = new ObjectMapper<ModelSubModel, DtoSubModel>();
            subMapper.Append(p => p.SubStrProp, p => p.SubStrProp);
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.Append(p => p.SubModel, p => p.SubModel, subMapper);
            var target = data.Map(mapper);

            var expected = new DtoModel
            {
                SubModel = null
            };

            target.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetArrayValueWhenMapQueryableComplexObjects()
        {
            Model data = new Model
            {
                QueryableSubModels = new ModelSubModel[] {
                    new ModelSubModel() { SubStrProp = "str1" },
                    null
                }.AsQueryable()
            };
            var subMapper = new ObjectMapper<ModelSubModel, DtoSubModel>();
            subMapper.Append(p => p.SubStrProp, p => p.SubStrProp);
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendCollection(p => p.SubModelArray, p => p.QueryableSubModels, subMapper);
            var target = data.Map(mapper);

            var expected = new DtoModel
            {
                SubModelArray = new DtoSubModel[] {
                    new DtoSubModel() { SubStrProp = "str1" },
                    null
                }
            };
            target.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public void ShouldGetArrayValueWhenMapQueryableComplexObjectsAsNull()
        {
            Model data = new Model
            {
                QueryableSubModels = null
            };
            var subMapper = new ObjectMapper<ModelSubModel, DtoSubModel>();
            subMapper.Append(p => p.SubStrProp, p => p.SubStrProp);
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendCollection(p => p.SubModelArray, p => p.QueryableSubModels, subMapper);
            var target = data.Map(mapper);

            var expected = new DtoModel
            {
                SubModelArray = null
            };
            target.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetArrayValueWhenMapEnumerableComplexObjects()
        {
            Model data = new Model
            {
                EnumerableSubModels = new ModelSubModel[] {
                    new ModelSubModel() { SubStrProp = "str1" },
                    null
                }
            };
            var subMapper = new ObjectMapper<ModelSubModel, DtoSubModel>();
            subMapper.Append(p => p.SubStrProp, p => p.SubStrProp);
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendCollection(p => p.SubModelArray, p => p.EnumerableSubModels, subMapper);
            var target = data.Map(mapper);

            var expected = new DtoModel
            {
                SubModelArray = new DtoSubModel[] {
                    new DtoSubModel() { SubStrProp = "str1" },
                    null
                }
            };
            target.Should().BeEquivalentTo(expected);
        }
        [Fact]
        public void ShouldGetArrayValueWhenMapEnumerableComplexObjectsAsNull()
        {
            Model data = new Model
            {
                QueryableSubModels = null
            };
            var subMapper = new ObjectMapper<ModelSubModel, DtoSubModel>();
            subMapper.Append(p => p.SubStrProp, p => p.SubStrProp);
            var mapper = new ObjectMapper<Model, DtoModel>();
            mapper.AppendCollection(p => p.SubModelArray, p => p.EnumerableSubModels, subMapper);
            var target = data.Map(mapper);

            var expected = new DtoModel
            {
                SubModelArray = null
            };
            target.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldGetTargetInstanceWhenUseDefaultMapper()
        {
            var datetime = DateTime.Now;
            var datetimeOffset = DateTimeOffset.Now;
            Model data = new Model
            {
                StrProp = "strprop",
                IntProp = 123,
                NullIntProp = 456,
                DateTimeProp = datetime,
                DateTimeOffsetProp = datetimeOffset,
                SubModel = new ModelSubModel
                {
                    IntProp = 234,
                    SubStrProp = "substrprop"
                }
            };
            var mapper = ObjectMapper<Model, DtoModel>.Default;
            var target = data.Map(mapper);
            var expected = new DtoModel
            {
                StrProp = "strprop",
                IntProp = 123,
                NullIntProp = 456,
                DateTimeProp = datetime,
                DateTimeOffsetProp = datetimeOffset,
                SubModel = new DtoSubModel
                {
                    IntProp = 234,
                    SubStrProp = "substrprop"
                }
            };
            target.Should().BeEquivalentTo(expected);
        }


        #region ShouldMapIntPropToNullableInt

        [Fact]
        public void ShouldMapIntPropToNullableInt()
        {
            var data = new Model2 { IntProp = 123 };

            var mapper = new ObjectMapper<Model2, Dto2>();
            mapper.Append(p => p.IntProp, p => (int?)p.IntProp);
            var target = data.Map(mapper);
            target.Should().BeEquivalentTo(new Dto2() { IntProp = 123 });
        }

        class Dto2
        {
            public int? IntProp { get; set; }
        }

        class Model2
        {
            public int IntProp { get; set; }
        }

        #endregion
        class DtoModel
        {
            public string StrProp { get; set; }
            public int IntProp { get; set; }
            public int? NullIntProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public DateTimeOffset DateTimeOffsetProp { get; set; }
            public DtoSubModel SubModel { get; set; }
            public DtoSubModel[] SubModelArray { get; set; }
        }
        class DtoSubModel
        {
            public string SubStrProp { get; set; }
            public int IntProp { get; set; }
        }

        class Model
        {
            public string StrProp { get; set; }
            public int IntProp { get; set; }
            public int? NullIntProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public DateTimeOffset DateTimeOffsetProp { get; set; }
            public ModelSubModel SubModel { get; set; }
            public IQueryable<ModelSubModel> QueryableSubModels { get; set; }
            public IEnumerable<ModelSubModel> EnumerableSubModels { get; set; }
        }
        public class ModelSubModel
        {
            public string SubStrProp { get; set; }
            public int IntProp { get; set; }
        }

        class ModelSource
        {
            public string StrProp { get; set; }
            public int IntProp { get; set; }
            public long LongProp { get; set; }
            public Guid GuidProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public DateTimeOffset DateTimeOffsetProp { get; set; }
            public int? NullableIntProp { get; set; }
            public Guid? NullableGuidProp { get; set; }
            public DateTime? NullableDateTimeProp { get; set; }
            public IA InterfaceObject { get; set; }
            public A1 ComplexObjectProp { get; set; }

            public string[] StrArray { get; set; }

            public List<string> StrList { get; set; }

            public IList<string> StrIList { get; set; }

            public IQueryable<string> StrQueryable { get; set; }

            public IEnumerable<string> StrEnumerable { get; set; }

            public IA[] InterfaceArray { get; set; }

            public List<IA> InterfaceList { get; set; }

            public IList<IA> InterfaceIList { get; set; }

            public IQueryable<IA> InterfaceQueryable { get; set; }

            public IEnumerable<IA> InterfaceEnumerable { get; set; }

            public A1[] ObjectArray { get; set; }

            public List<A1> ObjectList { get; set; }

            public IList<A1> ObjectIList { get; set; }

            public IQueryable<A1> ObjectQueryable { get; set; }

            public IEnumerable<A1> ObjectEnumerable { get; set; }

        }
        interface IA
        {
            public string SubProp { get; set; }
        }
        class A1 : IA
        {
            public string SubProp { get; set; }
        }

        class Dto
        {
            public class Basic
            {
                public string StrProp { get; set; }
                public int IntProp { get; set; }
                public long LongProp { get; set; }
                public Guid GuidProp { get; set; }
                public DateTime DateTimeProp { get; set; }
                public DateTimeOffset DateTimeOffsetProp { get; set; }
                public int? NullableIntProp { get; set; }
                public Guid? NullableGuidProp { get; set; }
                public DateTime? NullableDateTimeProp { get; set; }
            }
        }

    }
}
