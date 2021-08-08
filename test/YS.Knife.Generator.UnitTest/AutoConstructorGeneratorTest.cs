using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace YS.Knife.Generator.UnitTest
{
    
    public class AutoConstructorGeneratorTest : BaseGeneratorTest
    {
        [Theory]
        [InlineData("AutoConstructorCases/HappyCase.xml")]
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

    }


}
