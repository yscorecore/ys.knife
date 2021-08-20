using System.Reflection;
using Xunit;

namespace YS.Knife.Generator.UnitTest
{
    public class ConvertToGeneratorTest : BaseGeneratorTest
    {
        [Theory]
        [InlineData("ConvertToCases/HappyCase.xml")]
        [InlineData("ConvertToCases/ClassifyConversion.xml")]
        [InlineData("ConvertToCases/ClassToStruct.xml")]
        [InlineData("ConvertToCases/StructToClass.xml")]
        [InlineData("ConvertToCases/StructToStruct.xml")]
        [InlineData("ConvertToCases/IgnoreTargetProperty.xml")]
        [InlineData("ConvertToCases/IgnoreNullTargetProperty.xml")]
        [InlineData("ConvertToCases/IgnoreEmptyTargetProperty.xml")]
        [InlineData("ConvertToCases/IgnoreNotExistingTargetProperty.xml")]
        [InlineData("ConvertToCases/CustomerMappings.xml")]

        public void ShouldGenerateConverterClass(string testCaseFileName)
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



}
