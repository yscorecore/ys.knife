using System;
using System.Reflection;
using Xunit;

namespace YS.Knife.Generator.UnitTest
{
    
    public class SingletonPatternGeneratorTest : BaseGeneratorTest
    {
        [Theory]
        [InlineData("SingletonPatternCases/HappyCase.xml")]

        public void ShouldGenerateExpectSingletonPaitalClass(string testCaseFileName)
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
