using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YS.Knife
{
    [Generator]
    public class AutowiredGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new AutowiredSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is AutowiredSyntaxReceiver receiver))
                return;

            var codeWriter = new CodeWriter(context);

            codeWriter.ForeachClassSyntax(receiver.CandidateClasses, ProcessClass);

        }

        private CodeFile ProcessClass(INamedTypeSymbol classSymbol, CodeWriter codeWriter)
        {
            var fieldList = classSymbol.GetAllInstanceFieldsByAttribute(typeof(AutowiredAttribute)).ToList();
            if (fieldList.Count == 0) return null;

            CsharpCodeBuilder builder = new CsharpCodeBuilder();
            AppendNamespace(builder);
            AppendClassDefinition(builder);
            AppendPublicCtor(builder);
            builder.EndAllSegments();
            return new CodeFile
            {
                BasicName = string.Join(".", classSymbol.GetContainerClassChains().Select(p => p.Name)),
                Content = builder.ToString(),
            };

            void AppendNamespace(CsharpCodeBuilder codeBuilder)
            {
                if (!classSymbol.ContainingNamespace.IsGlobalNamespace)
                {
                    codeBuilder.AppendCodeLines($"namespace {classSymbol.ContainingNamespace.ToDisplayString()}");
                    codeBuilder.BeginSegment();
                }
            }

            void AppendClassDefinition(CsharpCodeBuilder codeBuilder)
            {
                var classSymbols = classSymbol.GetContainerClassChains();
                foreach (var parentClass in classSymbols)
                {
                    codeBuilder.AppendCodeLines($@"partial class {parentClass.GetClassSymbolDisplayText()}");
                    codeBuilder.BeginSegment();
                }
            }

            void AppendPublicCtor(CsharpCodeBuilder codeBuilder)
            {
                var nameMapper = new Dictionary<string, ISymbol>();
                // from parent class
                foreach (var paramSymbol in GetBaseTypeParameters())
                {
                    nameMapper[NewArgumentName(paramSymbol, nameMapper)] = paramSymbol;
                }
                // from current class
                foreach (var fieldSymbol in fieldList)
                {
                    nameMapper[NewArgumentName(fieldSymbol, nameMapper)] = fieldSymbol;
                }
                string args = string.Join(", ", nameMapper.Select(p => $"{GetSymbolTypeDisplayName(p.Value)} {p.Key}"));

                codeBuilder.AppendCodeLines($"public {classSymbol.Name}({args})");
                if (nameMapper.Values.OfType<IParameterSymbol>().Any())
                {
                    string baseArgs = string.Join(", ", nameMapper.Where(p => p.Value is IParameterSymbol).Select(p => $"{p.Value.Name}: {p.Key}"));
                    codeBuilder.AppendCodeLines($"    : base({baseArgs})");
                }


                codeBuilder.BeginSegment();
                // lines
                foreach (var kv in nameMapper)
                {
                    if (kv.Value is IFieldSymbol fieldSymbol)
                    {
                        codeBuilder.AppendCodeLines($"this.{fieldSymbol.Name} = {kv.Key};");
                    }
                }

                codeBuilder.EndSegment();

                string NewArgumentName(ISymbol symbol, IDictionary<string, ISymbol> ctx)
                {
                    string baseName = symbol.Name.ToCamelCase();
                    if (string.IsNullOrEmpty(baseName))
                    {
                        baseName = "args";
                    }

                    string newName = baseName;
                    int index = 1;
                    while (ctx.ContainsKey(newName))
                    {
                        newName = baseName + index++;
                    }

                    return newName;
                }

                ImmutableArray<IParameterSymbol> GetBaseTypeParameters()
                {
                    if (classSymbol.BaseType == null)
                    {
                        return ImmutableArray<IParameterSymbol>.Empty;
                    }

                    // if found Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructorAttribute, use it first
                    var primaryCtor = classSymbol.BaseType.Constructors
                        .FirstOrDefault(p => p.HasAttribute(
                            "Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructorAttribute"));
                    if (primaryCtor != null)
                    {
                        return primaryCtor.Parameters;
                    }

                    // use first minimum parameters
                    var ctor = classSymbol.BaseType.Constructors.OrderBy(method => method.Parameters.Length)
                        .FirstOrDefault();
                    if (ctor == null)
                    {
                        return ImmutableArray<IParameterSymbol>.Empty;
                    }

                    return ctor.Parameters;
                }
            }

            string GetSymbolTypeDisplayName(ISymbol symbol)
            {
                if (symbol is IFieldSymbol fieldSymbol)
                {
                    return fieldSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                }

                if (symbol is IParameterSymbol parameterSymbol)
                {
                    return parameterSymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                }
                // never go here
                throw new NotSupportedException();
            }
        }

        private class AutowiredSyntaxReceiver : ISyntaxReceiver
        {
            public IList<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    var hasInstanceFieldWithAttribute = classDeclarationSyntax.Members.OfType<FieldDeclarationSyntax>()
                        .Any(HasInstanceFieldAndDefinedAttribute);
                    if (hasInstanceFieldWithAttribute)
                    {
                        CandidateClasses.Add(classDeclarationSyntax);
                    }
                }

                bool HasInstanceFieldAndDefinedAttribute(FieldDeclarationSyntax fieldDeclarationSyntax)
                {
                    return fieldDeclarationSyntax.AttributeLists.Any() &&
                           !fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                           !fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.ConstKeyword);
                }
            }
        }
    }
}
