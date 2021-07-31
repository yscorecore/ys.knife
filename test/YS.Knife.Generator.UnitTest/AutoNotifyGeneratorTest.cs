using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Generator.UnitTest
{
    [TestClass]
    public class AutoNotifyGeneratorTest
    {
        [TestMethod]
        public void ShouldGenerateHappyCase()
        {
            var comp = CreateCompilation(Properties.Resources.AutoNotifyHappyCase);
            var newComp = RunGenerators(comp, out _, new AutoNotifyGenerator());

            var newFile = newComp.SyntaxTrees
              .Single(x => Path.GetFileName(x.FilePath).EndsWith(".AutoNotify.g.cs"));

            newFile.FilePath.Should().EndWith("Class1.AutoNotify.g.cs");

            newFile.GetText().ToString().Trim().Should().Be(Properties.Resources.AutoNotifyHappyExpected.Trim());
        }

        [TestMethod]
        public void ShouldGenerateNestedClassCase()
        {

            var comp = CreateCompilation(Properties.Resources.AutoNotifyNestedClass);
            var newComp = RunGenerators(comp, out _, new AutoNotifyGenerator());

            var newFile = newComp.SyntaxTrees
              .Single(x => Path.GetFileName(x.FilePath).EndsWith(".AutoNotify.g.cs"));

            newFile.FilePath.Should().EndWith("Class4.Class5.AutoNotify.g.cs");

            newFile.GetText().ToString().Trim().Should().Be(Properties.Resources.AutoNotifyNestedClassExpected.Trim());
        }
        [TestMethod]
        public void ShouldGenerateEmptyNamespaceCase()
        {

            var comp = CreateCompilation(Properties.Resources.AutoNotifyEmptyNameSpaceCase);
            var newComp = RunGenerators(comp, out _, new AutoNotifyGenerator());

            var newFile = newComp.SyntaxTrees
                .Single(x => Path.GetFileName(x.FilePath).EndsWith(".AutoNotify.g.cs"));

            newFile.FilePath.Should().EndWith("Class1.AutoNotify.g.cs");

            TrimLines(newFile.GetText().ToString()).Should().Be(TrimLines(Properties.Resources.AutoNotifyEmptyNameSpaceCaseExpected));
        }


        private static Compilation CreateCompilation(string source)
          => CSharpCompilation.Create("compilation",
            new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.CSharp10)) },
            new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location), MetadataReference.CreateFromFile(typeof(INotifyPropertyChanged).GetTypeInfo().Assembly.Location) },
            new CSharpCompilationOptions(OutputKind.ConsoleApplication));

        private static GeneratorDriver CreateDriver(params ISourceGenerator[] generators)
          => CSharpGeneratorDriver.Create(generators);

        private static Compilation RunGenerators(Compilation compilation, out ImmutableArray<Diagnostic> diagnostics, params ISourceGenerator[] generators)
        {
            CreateDriver(generators).RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out diagnostics);
            return newCompilation;
        }

        private string TrimLines(string text)
        {
            return string.Join(Environment.NewLine,
                text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(p=>p.Trim()).Where(string.IsNullOrEmpty));
        }
    }
}
