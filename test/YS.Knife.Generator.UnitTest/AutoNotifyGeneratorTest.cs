using System.ComponentModel;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Generator.UnitTest
{
    [TestClass]
    public class AutoNotifyGeneratorTest : BaseGeneratorTest
    {
        [DataTestMethod]
        [DataRow("AutoNotifyCases/HappyCase.xml")]
        [DataRow("AutoNotifyCases/GenericType.xml")]
        [DataRow("AutoNotifyCases/CombinAllPartials.xml")]
        [DataRow("AutoNotifyCases/NotifyPropertyChangedDefined.xml")]
        [DataRow("AutoNotifyCases/NotifyPropertyChangedInherited.xml")]
        [DataRow("AutoNotifyCases/NotifyPropertyChangedInheritedFromGenerator.xml")]
        [DataRow("AutoNotifyCases/NestedType.xml")]
        [DataRow("AutoNotifyCases/EmptyNamespace.xml")]
        [DataRow("AutoNotifyCases/SameNameInMultipleNamespace.xml")]
        [DataRow("AutoNotifyCases/NotifyPropertyChangedInheritedFromOtherAssembly.xml")]
        [DataRow("AutoNotifyCases/ComplexTypeFromCurrentSource.xml")]
        [DataRow("AutoNotifyCases/ComplexTypeFromCurrentSourceAndOtherNamespace.xml")]
        [DataRow("AutoNotifyCases/ComplexTypeFromOtherAssembly.xml")]
        public void ShouldGenerateExpectCodeFile(string testCaseFileName)
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

        public class BaseClass : INotifyPropertyChanged
        {

            public event PropertyChangedEventHandler PropertyChanged;


        }
    }
}
