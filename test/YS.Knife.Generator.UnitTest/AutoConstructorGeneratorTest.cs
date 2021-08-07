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
        [DataRow("AutoConstructorCases/HappyCase.xml")]
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


}
