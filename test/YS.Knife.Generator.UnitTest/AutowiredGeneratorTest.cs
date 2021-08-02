using System.ComponentModel;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Generator.UnitTest
{
    [TestClass]
    public class AutowiredGeneratorTest : BaseGeneratorTest
    {
        [DataTestMethod]
        [DataRow("AutowiredCases/HappyCase.xml")]
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
    }
}
