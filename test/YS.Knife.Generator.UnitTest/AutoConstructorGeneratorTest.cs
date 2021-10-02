using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
namespace YS.Knife.Generator.UnitTest
{

    public class AutoConstructorGeneratorTest : BaseGeneratorTest
    {
        [Theory]
        [InlineData("AutoConstructorCases/HappyCase.xml")]
        [InlineData("AutoConstructorCases/IgnoreField.xml")]
        [InlineData("AutoConstructorCases/EmptyNamespace.xml")]
        [InlineData("AutoConstructorCases/GenericType.xml")]
        [InlineData("AutoConstructorCases/CombinAllPartials.xml")]
        [InlineData("AutoConstructorCases/NestedType.xml")]
        [InlineData("AutoConstructorCases/SameNameInDifferentNamespace.xml")]
        [InlineData("AutoConstructorCases/CamelCase.xml")]
        [InlineData("AutoConstructorCases/InheriteOtherClass.xml")]
        [InlineData("AutoConstructorCases/InheriteClassFromOtherAssembly.xml")]
        [InlineData("AutoConstructorCases/ComplexTypeFromCurrentSource.xml")]
        [InlineData("AutoConstructorCases/ComplexTypeFromOtherAssembly.xml")]
        [InlineData("AutoConstructorCases/NullCheck.xml")]

        public void ShouldGenerateExpectConstructor(string testCaseFileName)
        {
            var assemblies = new[]
            {
                typeof(Binder).GetTypeInfo().Assembly,
                typeof(AutoConstructorAttribute).GetTypeInfo().Assembly,
                Assembly.GetExecutingAssembly()
            };
            base.ShouldGenerateExpectCodeFile(new AutoConstructorGenerator(), testCaseFileName, assemblies);
        }

        [Theory]
        [InlineData("AutoConstructorCases/DependencyInjection/HappyCase.xml")]
        [InlineData("AutoConstructorCases/DependencyInjection/IgnoreField.xml")]
        public void ShouldGenerateExpectConstructorWhenRefrenceDependencyInjection(string testCaseFileName)
        {
            var assemblies = new[]
            {
                typeof(Binder).GetTypeInfo().Assembly,
                typeof(AutoConstructorAttribute).GetTypeInfo().Assembly,
                typeof(ActivatorUtilitiesConstructorAttribute).GetTypeInfo().Assembly,
                Assembly.GetExecutingAssembly()
            };
            base.ShouldGenerateExpectCodeFile(new AutoConstructorGenerator(), testCaseFileName, assemblies);
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

    partial class Class1
    {
        private int value;

        private string str;

    }
    partial class Class1
    {
        public Class1(int value, string str)
        {
            this.value = value;
            this.str = str ?? throw new System.ArgumentNullException(nameof(str));
        }
    }
}
