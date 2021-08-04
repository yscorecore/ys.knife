using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YS.Knife
{
    [Generator]
    public class SingletonPatternGenerator:ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
           context.RegisterForSyntaxNotifications(()=>new SingletonPatternSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
           
            if (!(context.SyntaxReceiver is SingletonPatternSyntaxReceiver receiver))
                return;
            var codeWriter = new CodeWriter(context);

            codeWriter.ForeachClassSyntax(receiver.CandidateClasses, ProcessClass);
        }

        private CodeFile ProcessClass(INamedTypeSymbol classSymbol, CodeWriter codeWriter)
        {
            if (!classSymbol.HasAttribute(typeof(SingletonPatternAttribute)))
            {
                return null;
            }

            CsharpCodeBuilder codeBuilder = new CsharpCodeBuilder();
            AppendUsingLines(classSymbol,codeBuilder);
            AppendNamespace(classSymbol,codeBuilder);
            AppendClassDefinition(classSymbol, codeBuilder);
            AppendSingletonBody(classSymbol,codeBuilder);
            codeBuilder.EndAllSegments();
            return new CodeFile
            {
                BasicName = string.Join(".", classSymbol.GetContainerClassChains().Select(p => p.Name)),
                Content = codeBuilder.ToString(),
            };
        }

        void AppendUsingLines(INamedTypeSymbol _, CsharpCodeBuilder codeBuilder)
        {
            codeBuilder.AppendCodeLines("using System;");
        }

        void AppendNamespace(INamedTypeSymbol classSymbol, CsharpCodeBuilder codeBuilder)
        {
            if (!classSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                codeBuilder.AppendCodeLines($"namespace {classSymbol.ContainingNamespace.ToDisplayString()}");
                codeBuilder.BeginSegment();
            }
        }
        void AppendClassDefinition(INamedTypeSymbol classSymbol, CsharpCodeBuilder codeBuilder)
        {
            var classSymbols = classSymbol.GetContainerClassChains();
            foreach (var parentClass in classSymbols)
            {
                codeBuilder.AppendCodeLines($@"partial class {parentClass.GetClassSymbolDisplayText()}");
                codeBuilder.BeginSegment();
            }
        }

        void AppendSingletonBody(INamedTypeSymbol classSymbol, CsharpCodeBuilder codeBuilder)
        {
            var content = $@"private static readonly Lazy<{classSymbol.GetClassSymbolDisplayText()}> LazyInstance = new Lazy<{classSymbol.GetClassSymbolDisplayText()}>(() => new {classSymbol.GetClassSymbolDisplayText()}(), true);

private {classSymbol.Name}() {{ }}

public static {classSymbol.GetClassSymbolDisplayText()} Instance => LazyInstance.Value;";
            
            codeBuilder.AppendCodeLines(content);
        }

        private class SingletonPatternSyntaxReceiver : ISyntaxReceiver
        {
            public IList<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
                    !classDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                    classDeclarationSyntax.AttributeLists.Any())
                {
                    CandidateClasses.Add(classDeclarationSyntax);
                }
            }
        }
    }
}
