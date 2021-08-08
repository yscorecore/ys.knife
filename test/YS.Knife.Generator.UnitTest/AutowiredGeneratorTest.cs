using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace YS.Knife.Generator.UnitTest
{

    public class AutowiredGeneratorTest : BaseGeneratorTest
    {
        [Theory]
        [InlineData("AutowiredCases/HappyCase.xml")]
        [InlineData("AutowiredCases/GenericType.xml")]
        [InlineData("AutowiredCases/NestedType.xml")]
        [InlineData("AutowiredCases/CamelCase.xml")]
        [InlineData("AutowiredCases/EmptyNamespace.xml")]
        [InlineData("AutowiredCases/SameNameInDifferentNamespace.xml")]
        [InlineData("AutowiredCases/CombinAllPartials.xml")]
        [InlineData("AutowiredCases/InheriteOtherClass.xml")]
        [InlineData("AutowiredCases/InheriteClassFromOtherAssembly.xml")]
        [InlineData("AutowiredCases/ComplexTypeFromCurrentSource.xml")]
        [InlineData("AutowiredCases/ComplexTypeFromOtherAssembly.xml")]

        public void ShouldGenerateExpectPartailClass(string testCaseFileName)
        {
            var assemblies = new[]
            {
                typeof(Binder).GetTypeInfo().Assembly,
                typeof(AutowiredAttribute).GetTypeInfo().Assembly,
                Assembly.GetExecutingAssembly()
            };
            base.ShouldGenerateExpectCodeFile(new AutowiredGenerator(), testCaseFileName, assemblies);
        }

        public class BaseClassWithEmptyCtor
        {




        }
        public class BaseClassWith2Ctors
        {
            public BaseClassWith2Ctors(string strValue, int intValue)
            {

            }

            public BaseClassWith2Ctors(string value)
            {

            }

        }
        public class BaseClassWithAttributeInCtor
        {
            [ActivatorUtilitiesConstructor()]
            public BaseClassWithAttributeInCtor(string strValue, int intValue)
            {

            }

            public BaseClassWithAttributeInCtor(string strValue)
            {

            }

        }
        public class Model1
        {

        }
        public class Model2
        {

        }
    }
}
