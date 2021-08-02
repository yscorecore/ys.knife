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

            foreach (var clazzSymbol in codeWriter.GetAllClassSymbolsIgnoreRepeated(receiver.CandidateClasses))
            {
                var fieldList = clazzSymbol.GetAllInstanceFieldsByAttribute(typeof(AutowiredAttribute)).ToList();
                if (fieldList.Any())
                {
                    var codeFile = ProcessClass(clazzSymbol, fieldList, codeWriter);
                    codeWriter.WriteCodeFile(codeFile);
                }
            }
        }

        private CodeFile ProcessClass(INamedTypeSymbol classSymbol, List<IFieldSymbol> fieldList, CodeWriter codeWriter)
        {
            CsharpCodeBuilder builder = new CsharpCodeBuilder();
            AppendUsing(builder);
            AppendNamespace(builder);
            AppendClassDefinition(builder);
            AppendPublicCtor(builder);
            builder.EndAllSegments();
            return new CodeFile
            {
                BasicName = string.Join(".", classSymbol.GetParentClassChains().Select(p => p.Name)),
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

            void AppendUsing(CsharpCodeBuilder codeBuilder)
            {
                var allNamespaces = new HashSet<string>();
                foreach (var field in fieldList)
                {
                    if (!field.Type.ContainingNamespace.IsGlobalNamespace)
                    {
                        allNamespaces.Add(field.Type.ContainingNamespace.ToDisplayString());
                    }
                }

                allNamespaces.Remove(classSymbol.ContainingNamespace.ToDisplayString());

                foreach (var usingNamespace in allNamespaces.OrderBy(p => p))
                {
                    codeBuilder.AppendCodeLines($"using {usingNamespace};");
                }
            }

            void AppendClassDefinition(CsharpCodeBuilder codeBuilder)
            {
                var classSymbols = classSymbol.GetParentClassChains();
                foreach (var parentClass in classSymbols)
                {
                    codeBuilder.AppendCodeLines($@"partial class {parentClass.GetClassSymbolDisplayText()}");
                    codeBuilder.BeginSegment();
                }
            }

            void AppendPublicCtor(CsharpCodeBuilder codeBuilder)
            {
                var nameMapper = new Dictionary<string, ISymbol>();
                // from current class
                foreach (var fieldSymbol in fieldList)
                {
                    nameMapper[NewArgumentName(fieldSymbol, nameMapper)] = fieldSymbol;
                }

                // from parent class
                foreach (var paramSymbol in GetBaseTypeParameters())
                {
                    nameMapper[NewArgumentName(paramSymbol, nameMapper)] = paramSymbol;
                }

                string args = string.Join(", ", nameMapper.Select(p => $"{GetSymbolTypeDisplayName(p.Value)} {p.Key}"));

                codeBuilder.AppendCodeLines($"public {classSymbol.Name}({args})");
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
                    return fieldSymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
                }

                if (symbol is IParameterSymbol parameterSymbol)
                {
                    return parameterSymbol.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
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
