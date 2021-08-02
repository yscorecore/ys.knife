﻿using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace YS.Knife.Generator.UnitTest
{
    public class BaseGeneratorTest
    {
        protected void ShouldGenerateExpectCodeFile(ISourceGenerator generator, string testCaseFileName, params Assembly[] assemblies)
        {
            XDocument xmlFile = XDocument.Load(testCaseFileName);
            var codes = xmlFile.XPathSelectElements("case/input/code")
                .Select(prop => prop.Value).ToArray();
            var newComp = RunGenerators(CreateCompilation(codes, assemblies), out var warningAndErrors,
                generator);

            warningAndErrors.Should().BeEmpty();

            var outputs = xmlFile.XPathSelectElements("case/output/code")
                .Select(prop => (File: prop.Attribute("file").Value, Content: prop.Value));

            foreach (var output in outputs)
            {
                newComp.SyntaxTrees.Should()
                    .ContainSingle(x => Path.GetFileName(x.FilePath).Equals(output.File),
                        $"output file '{output.File}' not generated")
                    .Which.GetText().ToString().NormalizeCode().Should().BeEquivalentTo(output.Content.NormalizeCode());
            }
        }

        private static Compilation CreateCompilation(string[] sources, Assembly[] assemblies)
        {
            var allSources = sources
                .Select(p => CSharpSyntaxTree.ParseText(p, new CSharpParseOptions(LanguageVersion.CSharp10)));
            var allReferenceAssemblies =
                assemblies.Select(p => MetadataReference.CreateFromFile(p.Location));
            return CSharpCompilation.Create("tempassembly",
                allSources,
                allReferenceAssemblies,
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));
        }

        private static GeneratorDriver CreateDriver(params ISourceGenerator[] generators)
            => CSharpGeneratorDriver.Create(generators);

        private static Compilation RunGenerators(Compilation compilation, out ImmutableArray<Diagnostic> diagnostics,
            params ISourceGenerator[] generators)
        {
            CreateDriver(generators)
                .RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out diagnostics);
            return newCompilation;
        }
    }
}
