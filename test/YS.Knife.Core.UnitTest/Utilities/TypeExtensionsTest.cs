using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
namespace YS.Knife.Utilities
{

    public class TypeExtensionsTest
    {
        [InlineData(typeof(int))]
        [InlineData(typeof(double))]
        [InlineData(typeof(DateTime))]
        [InlineData(typeof(object))]
        [InlineData(typeof(ICollection<>))]
        [Theory]
        public void ShouldNotBeGenericEnumerableType(Type type)
        {
            type.IsGenericEnumerable().Should().Be(false);
        }

        [InlineData(typeof(string))]// string is IEnumerable<string>
        [InlineData(typeof(List<int>))]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(ICollection<int>))]
        [Theory]
        public void ShouldBeGenericEnumerableType(Type type)
        {
            type.IsGenericEnumerable().Should().Be(true);
        }

    }
}
