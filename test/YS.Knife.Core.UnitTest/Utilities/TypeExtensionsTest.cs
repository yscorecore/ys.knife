using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
namespace YS.Knife.Utilities
{
    [TestClass]
    public class TypeExtensionsTest
    {
        [DataRow(typeof(int))]
        [DataRow(typeof(double))]
        [DataRow(typeof(DateTime))]
        [DataRow(typeof(object))]
        [DataRow(typeof(ICollection<>))]
        [DataTestMethod]
        public void ShouldNotBeGenericEnumerableType(Type type)
        {
            type.IsGenericEnumerable().Should().Be(false);
        }
        
        [DataRow(typeof(string))]// string is IEnumerable<string>
        [DataRow(typeof(List<int>))]
        [DataRow(typeof(int[]))]
        [DataRow(typeof(ICollection<int>))]
        [DataTestMethod]
        public void ShouldBeGenericEnumerableType(Type type)
        {
            type.IsGenericEnumerable().Should().Be(true);
        }
        
    }
}
