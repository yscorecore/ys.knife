using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Generator.UnitTest
{
    [TestClass]
    public class SingletonPatternGeneratorTest: BaseGeneratorTest
    {
        [DataTestMethod]
        [DataRow("SingletonPatternCases/HappyCase.xml")]

        public void ShouldGenerateExpectCodeFile(string testCaseFileName)
        {
            var assemblies = new[]
            {
                typeof(Binder).GetTypeInfo().Assembly,
                typeof(SingletonPatternAttribute).GetTypeInfo().Assembly,
                Assembly.GetExecutingAssembly()
            };
            base.ShouldGenerateExpectCodeFile(new SingletonPatternGenerator(), testCaseFileName, assemblies);
        }
    }
}
