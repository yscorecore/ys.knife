using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace YS.Knife.Generator.UnitTest
{
    public class ConvertToGeneratorTest : BaseGeneratorTest
    {
        [Theory]
        [InlineData("ConvertToCases/HappyCase.xml")]

        public void ShouldGenerateExpectSingletonPaitalClass(string testCaseFileName)
        {
            var assemblies = new[]
            {
                typeof(Binder).GetTypeInfo().Assembly,
                typeof(ConvertToAttribute).GetTypeInfo().Assembly,
                Assembly.GetExecutingAssembly()
            };
            base.ShouldGenerateExpectCodeFile(new ConvertToGenerator(), testCaseFileName, assemblies);
        }
    }

    public class UserInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? Age { get; set; }

        public int? OtherProp { get; set; }

    }

    public partial class TUser
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? Age { get; set; }
    }
    

}
