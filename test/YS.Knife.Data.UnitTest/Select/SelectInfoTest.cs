using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Data.Select
{
    [TestClass]
    public class SelectInfoTest
    {
        [TestMethod]
        public void should_get_empty_string_when_to_string_given_non_items()
        {
            var select = new SelectInfo()
            {
                Items = null
            };
            select.ToString().Should().Be(string.Empty);
        }
        [TestMethod]
        public void should_join_items_when_to_string_given_simple_items()
        {
            var select = new SelectInfo()
            {
                Items = new List<SelectItem>
                {
                     new SelectItem{ Name="a" },
                     new SelectItem{ Name="b" }
                }
            };
            select.ToString().Should().Be("a,b");
        }

        [TestMethod]
        public void should_join_items_when_to_string_given_sub_items()
        {
            var select = new SelectInfo()
            {
                Items = new List<SelectItem>
                {
                     new SelectItem{ Name="a" },
                     new SelectItem{ Name="b",SubItems= new List<SelectItem>{ new SelectItem {  Name="c"} } },
                     new SelectItem{ Name="d" },
                }
            };
            select.ToString().Should().Be("a,b(c),d");
        }
        [TestMethod]
        public void should_join_items_when_to_string_given_collection_filter()
        {
            var select = new SelectInfo()
            {
                Items = new List<SelectItem>
                {
                     new SelectItem{ Name="a" },
                     new SelectItem{ Name="b",CollectionFilter= FilterInfo2.Parse("c=123")},
                     new SelectItem{ Name="d" },
                }
            };
            select.ToString().Should().Be("a,b{c == 123},d");
        }
        [TestMethod]
        public void should_join_items_when_to_string_given_collection_order()
        {
            var select = new SelectInfo()
            {
                Items = new List<SelectItem>
                {
                     new SelectItem{ Name="a" },
                     new SelectItem{ Name="b",CollectionOrder= OrderInfo.Parse("c,-d,+e")},
                     new SelectItem{ Name="f" },
                }
            };
            select.ToString().Should().Be("a,b{+c,-d,+e},f");
        }
        [TestMethod]
        public void should_join_items_when_to_string_given_collection_limit()
        {
            var select = new SelectInfo()
            {
                Items = new List<SelectItem>
                {
                     new SelectItem{ Name="a" },
                     new SelectItem{ Name="b",CollectionLimit=new LimitInfo(1,5)},
                     new SelectItem{ Name="f" },
                }
            };
            select.ToString().Should().Be("a,b{1,5},f");
        }
        [TestMethod]
        public void should_join_items_when_to_string_given_all_sub_sfuff()
        {
            var select = new SelectInfo()
            {
                Items = new List<SelectItem>
                {
                     new SelectItem{ Name="a" },
                     new SelectItem
                     {
                         Name="b",
                         CollectionFilter=FilterInfo2.Parse("(c>1)and(d<2)"),
                         CollectionOrder=OrderInfo.Parse("e,-f,+g"),
                         CollectionLimit=new LimitInfo(1,5),
                         SubItems = new List<SelectItem> {
                             new SelectItem{Name="h"},
                             new SelectItem{Name="i"}
                         }
                     },
                     new SelectItem{ Name="j" },
                }
            };
            select.ToString().Should().Be("a,b{(c > 1) and (d < 2),+e,-f,+g,1,5}(h,i),j");
        }

        //[TestMethod]
        //public void should_deserialize_from_json_string()
        //{
        //    var jsonText = "{\"select\":\"a,b,c\"}";
        //    var selectWrap = Json.DeSerialize<SelectWrap>(jsonText);
        //    selectWrap.Select.Should().BeEquivalentTo(SelectInfo.Parse("a,b,c"));
        //}
        class SelectWrap
        {
            public SelectInfo Select { get; set; }
        }
    }
}
