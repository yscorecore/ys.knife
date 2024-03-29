﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace YS.Knife.Data.Mapper
{

    public class MapperExtensionsTest
    {
        [Fact]
        public void ShouldMapSingleItemFromSourceTypeToTargetType()
        {
            var input = new Model1() { StrVal = "str" };
            var actual = input.DynamicMap<Model2>();
            actual.Should().BeEquivalentTo(new Model2 { StrVal = "str" });
        }

        [Fact]
        public void ShouldMapSingleItemFromAnonymousTypeToTargetType()
        {
            var input = new { StrVal = "str" };
            var actual = input.DynamicMap<Model2>();
            actual.Should().BeEquivalentTo(new Model2 { StrVal = "str" });
        }

        [Fact]
        public void ShouldMapEnumerableFromSourceTypeToTargetType()
        {
            var input = new List<Model1> { new Model1 { StrVal = "str1" } };
            var actual = input.DynamicMap<Model2>().ToList();
            var expected = new List<Model2> { new Model2 { StrVal = "str1" } };
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldMapEnumerableFromAnonymousTypeToTargetType()
        {
            var input = new[] { new { StrVal = "str1" }, new { StrVal = "str2" } };
            var actual = input.DynamicMap<Model2>().ToList();
            var expected = new List<Model2> { new Model2 { StrVal = "str1" }, new Model2 { StrVal = "str2" } };
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldMapQueryableFromSourceTypeToTargetType()
        {
            var input = new List<Model1> { new Model1 { StrVal = "str1" } }.AsQueryable();
            var actual = input.DynamicMap<Model2>().ToList();
            var expected = new List<Model2> { new Model2 { StrVal = "str1" } };
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ShouldMapQueryableFromAnonymousTypeToTargetType()
        {
            var input = new[] { new { StrVal = "str1" }, new { StrVal = "str2" } }.AsQueryable();
            var actual = input.DynamicMap<Model2>().ToList();
            var expected = new List<Model2> { new Model2 { StrVal = "str1" }, new Model2 { StrVal = "str2" } };
            actual.Should().BeEquivalentTo(expected);
        }

        class Model1
        {
            public string StrVal { get; set; }
        }

        class Model2
        {
            public string StrVal { get; set; }
        }
    }
}
