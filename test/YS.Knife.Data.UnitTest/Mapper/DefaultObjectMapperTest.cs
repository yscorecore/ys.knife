using System.Collections.Generic;
using System.Linq;
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

        #region ShouldMapNullableValueList

        [TestMethod]
        public void ShouldMapNullableValueList()
        {
            var data = new Model5
            {
                InnerList = new List<int>{1,2,3}
            };
            var mapper = new ObjectMapper<Model5,Dto5>();
           
            mapper.AppendCollection(p=>p.InnerList,p=>p.InnerList.Select(item=>(int?)item).AsQueryable());

            var target = data.Map(mapper);
            target.Should().BeEquivalentTo(new Dto5()
            {
                InnerList = new int?[]{1,2,3}
            });
        }


        class Dto5
        {
            public int?[] InnerList { get; set; }
        }

      

        class Model5
        {
            public List<int> InnerList { get; set; }
        }

        #endregion
        
        #region ShouldMapAssignList
        [TestMethod]
        public void ShouldMapAssignList()
        {
            var data = new Model6()
            {
                DataList = new List<Data6>
                {
                    new Data6("abc"),
                    null,
                    new Data6("bcd")
                }
            };


            var target = data.Map(ObjectMapper<Model6, Dto6>.Default);
            target.DataList.Should().BeEquivalentTo(data.DataList);
        }


        class Dto6
        {
            public List<IData6> DataList { get; set; }
        }

        interface IData6
        {
            public string StrProp { get; set; }
        }
        class Data6:IData6
        {
            public Data6(string val)
            {
                this.StrProp = val;
            }
            public string StrProp { get; set; }
        }

      

        class Model6
        {
            public List<Data6> DataList { get; set; }
        }
        
        #endregion
    }
}
