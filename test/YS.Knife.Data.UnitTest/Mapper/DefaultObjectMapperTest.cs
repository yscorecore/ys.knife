using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Mappers;

namespace YS.Knife.Data.UnitTest.Mapper
{
    [TestClass]
    public class DefaultObjectMapperTest
    {
        #region ShouldMapStringProp

        [TestMethod]
        public void ShouldMapStringProp()
        {
            var data = new Model {StrProp = "str"};


            var target = data.Map(ObjectMapper<Model, Dto>.Default);
            target.Should().BeEquivalentTo(new Dto() {StrProp = "str"});
        }

        class Dto
        {
            public string StrProp { get; set; }
        }

        class Model
        {
            public string StrProp { get; set; }
        }

        #endregion

        #region ShouldMapIntPropToNullableInt

        [TestMethod]
        public void ShouldMapIntPropToNullableInt()
        {
            var data = new Model2 {IntProp = 123};


            var target = data.Map(ObjectMapper<Model2, Dto2>.Default);
            target.Should().BeEquivalentTo(new Dto2() {IntProp = 123});
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

        #region ShouldMapComplexObject

        [TestMethod]
        public void ShouldMapComplexObjectWhenInnerPropIsNull()
        {
            var data = new Model3 {Inner = null};


            var target = data.Map(ObjectMapper<Model3, Dto3>.Default);
            target.Should().BeEquivalentTo(new Dto3() {Inner = null});
        }

        [TestMethod]
        public void ShouldMapComplexObjectWhenInnerPropHasValue()
        {
            var data = new Model3 {Inner = new InnerModel3() {InnerStrProp = "str"}};


            var target = data.Map(ObjectMapper<Model3, Dto3>.Default);
            target.Should().BeEquivalentTo(new Dto3() {Inner = new InnerDto3() {InnerStrProp = "str"}});
        }

        class Dto3
        {
            public InnerDto3 Inner { get; set; }
        }

        class InnerDto3
        {
            public string InnerStrProp { get; set; }
        }

        class Model3
        {
            public InnerModel3 Inner { get; set; }
        }

        class InnerModel3
        {
            public string InnerStrProp { get; set; }
        }

        #endregion

        #region ShouldMapComplexObjectList

        [TestMethod]
        public void ShouldMapComplexObjectList()
        {
            var data = new Model4
            {
                Inner = new[]
                {
                    new InnerModel4 {InnerStrProp = "str1"}, null, new InnerModel4() {InnerStrProp = "str2"}
                }
            };


            var target = data.Map(ObjectMapper<Model4, Dto4>.Default);
            target.Should().BeEquivalentTo(new Dto4()
            {
                Inner = new List<InnerDto4>
                {
                    new InnerDto4 {InnerStrProp = "str1"}, null, new InnerDto4 {InnerStrProp = "str2"}
                }
            });
        }


        class Dto4
        {
            public List<InnerDto4> Inner { get; set; }
        }

        class InnerDto4
        {
            public string InnerStrProp { get; set; }
        }

        class Model4
        {
            public InnerModel4[] Inner { get; set; }
        }

        class InnerModel4
        {
            public string InnerStrProp { get; set; }
        }

        #endregion
    }
}
