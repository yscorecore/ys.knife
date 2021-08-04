using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Generator.UnitTest
{
    [TestClass]
    public class AutoConstructorGeneratorTest : BaseGeneratorTest
    {
        [DataTestMethod]
        //[DataRow("AutoConstructorCases/HappyCase.xml")]
        [DataRow("AutoConstructorCases/CustomizeFieldExtension.xml")]
        public void ShouldGenerateExpectCodeFile(string testCaseFileName)
        {
            var assemblies = new[]
            {
                typeof(Binder).GetTypeInfo().Assembly,
                typeof(AutoConstructorAttribute).GetTypeInfo().Assembly,
                Assembly.GetExecutingAssembly()
            };
            base.ShouldGenerateExpectCodeFile(new AutoConstructorGenerator(), testCaseFileName, assemblies);
        }

    }

    public class ListFieldAttribute : AutoConstructorExtensionFieldAttribute
    {
        public ListFieldAttribute() : base("children", "System.Collections.IList", "System.Collections.Generic.IList`1")
        {

        }

        [ModuleInitializer]

        public static void Initializer()
        {
           AutoConstructorGenerator.AddExtensionField(new ListFieldAttribute());
        }
    }
}
