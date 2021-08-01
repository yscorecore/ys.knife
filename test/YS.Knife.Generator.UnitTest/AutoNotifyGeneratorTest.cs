using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Generator.UnitTest
{
    [TestClass]
    public class AutoNotifyGeneratorTest
    {
        [DataTestMethod]
        [DataRow("AutoNotifyCases/HappyCase.xml")]

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
            XDocument xmlFile = XDocument.Load(testCaseFileName);
            var codes = xmlFile.XPathSelectElements("case/input/code")
                        .Select(prop => prop.Value);
            var newComp = RunGenerators(CreateCompilation(codes.ToArray()), out var warningAndErrors, new AutoNotifyGenerator());

            warningAndErrors.Should().BeEmpty();

            var outputs = xmlFile.XPathSelectElements("case/output/code")
                        .Select(prop => (File: prop.Attribute("file").Value, Content: prop.Value));

            foreach (var output in outputs)
            {

                newComp.SyntaxTrees.Should()
                    .ContainSingle(x => Path.GetFileName(x.FilePath).Equals(output.File), $"output file '{output.File}' not generated")
                    .Which.GetText().ToString().NormalizeCode().Should().BeEquivalentTo(output.Content.NormalizeCode());
            }
          
        }

        private static Compilation CreateCompilation(params string[] sources)
        {
            var allSources = sources.Select(p => CSharpSyntaxTree.ParseText(p, new CSharpParseOptions(LanguageVersion.CSharp10))).ToArray();
            return CSharpCompilation.Create("compilation",
                allSources,
                  new[] {
                    MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(INotifyPropertyChanged).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(AutoNotifyAttribute).GetTypeInfo().Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location)
                  },
                  new CSharpCompilationOptions(OutputKind.ConsoleApplication));

        }

        private static GeneratorDriver CreateDriver(params ISourceGenerator[] generators)
          => CSharpGeneratorDriver.Create(generators);

        private static Compilation RunGenerators(Compilation compilation, out ImmutableArray<Diagnostic> diagnostics, params ISourceGenerator[] generators)
        {
            CreateDriver(generators).RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out diagnostics);
            return newCompilation;
        }
    }
}
