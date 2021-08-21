using System.ComponentModel;
using System.Reflection;
using Xunit;

namespace YS.Knife.Generator.UnitTest
{

    public class AutoNotifyGeneratorTest : BaseGeneratorTest
    {
        [Theory]
        [InlineData("AutoNotifyCases/HappyCase.xml")]
        [InlineData("AutoNotifyCases/CustomPropertyName.xml")]
        [InlineData("AutoNotifyCases/GenericType.xml")]
        [InlineData("AutoNotifyCases/CombinAllPartials.xml")]
        [InlineData("AutoNotifyCases/NotifyPropertyChangedDefined.xml")]
        [InlineData("AutoNotifyCases/NotifyPropertyChangedInherited.xml")]
        [InlineData("AutoNotifyCases/NotifyPropertyChangedInheritedFromGenerator.xml")]
        [InlineData("AutoNotifyCases/NestedType.xml")]
        [InlineData("AutoNotifyCases/EmptyNamespace.xml")]
        [InlineData("AutoNotifyCases/SameNameInMultipleNamespace.xml")]
        [InlineData("AutoNotifyCases/NotifyPropertyChangedInheritedFromOtherAssembly.xml")]
        [InlineData("AutoNotifyCases/ComplexTypeFromCurrentSource.xml")]
        [InlineData("AutoNotifyCases/ComplexTypeFromCurrentSourceAndOtherNamespace.xml")]
        [InlineData("AutoNotifyCases/ComplexTypeFromOtherAssembly.xml")]
        public void ShouldGenerateExpectPartailClass(string testCaseFileName)
        {
            var assemblies = new[]
            {
                typeof(Binder).GetTypeInfo().Assembly,
                typeof(INotifyPropertyChanged).GetTypeInfo().Assembly,
                typeof(AutoNotifyAttribute).GetTypeInfo().Assembly,
                Assembly.GetExecutingAssembly()
            };
            base.ShouldGenerateExpectCodeFile(new AutoNotifyGenerator(), testCaseFileName, assemblies);
        }

        [Theory]
        [InlineData("AutoNotifyCases/Error.EmptyPropertyName.xml")]
        [InlineData("AutoNotifyCases/Error.InvalidPropertyName.xml")]
        [InlineData("AutoNotifyCases/Error.PropertyNameEqualFieldName.xml")]
        public void ShouldReportDigError(string testCaseFileName)
        {
            var assemblies = new[]
            {
                typeof(Binder).GetTypeInfo().Assembly,
                typeof(INotifyPropertyChanged).GetTypeInfo().Assembly,
                typeof(AutoNotifyAttribute).GetTypeInfo().Assembly,
                Assembly.GetExecutingAssembly()
            };
            base.ShouldReportDiagnostic(new AutoNotifyGenerator(), testCaseFileName, assemblies);
        }

        public class BaseClass : INotifyPropertyChanged
        {

            public event PropertyChangedEventHandler PropertyChanged;


        }
    }
}
