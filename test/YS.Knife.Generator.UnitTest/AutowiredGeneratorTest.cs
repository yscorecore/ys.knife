using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Generator.UnitTest
{
    [TestClass]
    public class AutowiredGeneratorTest : BaseGeneratorTest
    {
        [DataTestMethod]
        [DataRow("AutowiredCases/HappyCase.xml")]
        [DataRow("AutowiredCases/GenericType.xml")]
        [DataRow("AutowiredCases/NestedType.xml")]
        [DataRow("AutowiredCases/CamelCase.xml")]
        [DataRow("AutowiredCases/EmptyNamespace.xml")]
        [DataRow("AutowiredCases/SameNameInDifferentNamespace.xml")]
        [DataRow("AutowiredCases/CombinAllPartials.xml")]
        [DataRow("AutowiredCases/InheriteOtherClass.xml")]
        [DataRow("AutowiredCases/InheriteClassFromOtherAssembly.xml")]
        [DataRow("AutowiredCases/ComplexTypeFromCurrentSource.xml")]
        [DataRow("AutowiredCases/ComplexTypeFromOtherAssembly.xml")]
        
        public void ShouldGenerateExpectCodeFile(string testCaseFileName)
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
