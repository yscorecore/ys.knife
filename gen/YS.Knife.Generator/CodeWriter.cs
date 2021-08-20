using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace YS.Knife
{
    class CodeWriter
    {
        public static CSharpParseOptions CSharpOptions = new CSharpParseOptions(LanguageVersion.CSharp9);
        public CodeWriter(GeneratorExecutionContext context)
        {
            Context = context;
            this.Compilation = context.Compilation;
        }
        public string CodeFileSuffix { get; set; } = "g.cs";
        public Compilation Compilation { get; private set; }
        public GeneratorExecutionContext Context { get; }

        private Dictionary<string, int> fileNames = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

        public void WriteCodeFile(CodeFile codeFile)
        {
            if (codeFile == null) return;

            fileNames.TryGetValue(codeFile.BasicName, out var i);
            var name = i == 0 ? codeFile.BasicName : $"{codeFile.BasicName}.{i + 1}";
            fileNames[codeFile.BasicName] = i + 1;

            this.Context.AddSource($"{name}.{CodeFileSuffix}", codeFile.Content);

            this.Compilation = this.Compilation.AddSyntaxTrees(
               CSharpSyntaxTree.ParseText(SourceText.From(codeFile.Content, Encoding.UTF8)));
        }
    }
    static class CodeWriterExtensions
    {
        class ClassSyntaxCachedInfo
        {
            public ClassDeclarationSyntax Syntax { get; set; }

            public INamedTypeSymbol NameTypedSymbol { get; set; }

            public string QualifiedName { get; set; }

            public bool Handled { get; set; }
        }
        public static void ForeachClassSyntax(this CodeWriter codeWriter, IEnumerable<ClassDeclarationSyntax> classSyntax, Func<INamedTypeSymbol, CodeWriter, CodeFile> codeFileFactory)
        {
            _ = codeFileFactory ?? throw new ArgumentNullException(nameof(codeFileFactory));
            var dic = new Dictionary<string, ClassSyntaxCachedInfo>();

            foreach (var clazz in classSyntax ?? Enumerable.Empty<ClassDeclarationSyntax>())
            {
                SemanticModel model = codeWriter.Compilation.GetSemanticModel(clazz.SyntaxTree);
                var clazzSymbol = model.GetDeclaredSymbol(clazz);

                var digs = model.GetDeclarationDiagnostics();
                if (digs.Length > 0)
                {
                    // Log warning..
                }

                var qualifiedName = clazzSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                dic[qualifiedName] = new ClassSyntaxCachedInfo
                {
                    Handled = false,
                    NameTypedSymbol = clazzSymbol,
                    Syntax = clazz,
                    QualifiedName = qualifiedName,
                };
            }

            foreach (var classCachedInfo in dic.Values)
            {

                Visit(classCachedInfo);
            }
            void Visit(ClassSyntaxCachedInfo value)
            {
                if (value.Handled) return;
                if (value.NameTypedSymbol.BaseType != null)
                {
                    var baseQualifiedName = value.NameTypedSymbol.BaseType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    if (dic.TryGetValue(baseQualifiedName, out var baseCachedInfo))
                    {
                        Visit(baseCachedInfo);
                    }
                }
                SemanticModel model = codeWriter.Compilation.GetSemanticModel(value.Syntax.SyntaxTree);
                var clazzSymbol = model.GetDeclaredSymbol(value.Syntax);
                codeWriter.WriteCodeFile(codeFileFactory(clazzSymbol, codeWriter));
                value.Handled = true;
            }
        }
    }
}
