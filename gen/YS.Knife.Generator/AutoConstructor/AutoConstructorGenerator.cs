using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YS.Knife
{
    [Generator]
    public class AutoConstructorGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new AutoConstructorSyntaxReceiver());
        }
        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is AutoConstructorSyntaxReceiver receiver))
            {
                return;
            }
            var codeWriter = new CodeWriter(context);

            codeWriter.ForeachClassSyntax(receiver.CandidateClasses, ProcessClass);
        }

        private CodeFile ProcessClass(INamedTypeSymbol classSymbol, CodeWriter codeWriter)
        {
            if (!classSymbol.HasAttribute(typeof(AutoConstructorAttribute)))
            {
                return null;
            }
            var nameMapper = GetSymbolNameMapper(classSymbol);
            if (!nameMapper.Any())
            {
                return null;
            }
            CsharpCodeBuilder builder = new CsharpCodeBuilder();
            AppendNamespace(classSymbol, builder);
            AppendClassDefinition(classSymbol, builder);
            AppendPublicCtor(classSymbol, nameMapper, builder);
            builder.EndAllSegments();
            return new CodeFile
            {
                BasicName = string.Join(".", classSymbol.GetContainerClassChains().Select(p => p.Name)),
                Content = builder.ToString(),
            };

        }


        IDictionary<string, ISymbol> GetSymbolNameMapper(INamedTypeSymbol classSymbol)
        {
            var nameMapper = new Dictionary<string, ISymbol>();

            foreach (var baseParam in GetBaseTypeParameters())
            {
                nameMapper[NewArgumentName(baseParam, nameMapper)] = baseParam;
            }
            foreach (var field in GetInstanceFields())
            {
                nameMapper[NewArgumentName(field, nameMapper)] = field;
            }
            foreach (var property in GetInstanceProperties())
            {
                nameMapper[NewArgumentName(property, nameMapper)] = property;
            }
            return nameMapper;

            IEnumerable<IFieldSymbol> GetInstanceFields()
            {
                return classSymbol.GetMembers().OfType<IFieldSymbol>()
                    .Where(p => !p.IsStatic && !p.IsConst && p.CanBeReferencedByName && !p.HasAttribute(typeof(AutoConstructorIgnoreAttribute)));
            }
            IEnumerable<IPropertySymbol> GetInstanceProperties()
            {
                return classSymbol.GetMembers().OfType<IPropertySymbol>()
                    .Where(p => !p.IsStatic && !p.IsReadOnly && p.CanBeReferencedByName && !p.HasAttribute(typeof(AutoConstructorIgnoreAttribute)));
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
        void AppendPublicCtor(INamedTypeSymbol classSymbol, IDictionary<string, ISymbol> nameMapper, CsharpCodeBuilder codeBuilder)
        {

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
                if (symbol is IPropertySymbol propertySymbol)
                {
                    return propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                }
                // never go here
                throw new NotSupportedException();
            }

        }
        private class AutoConstructorSyntaxReceiver : ISyntaxReceiver
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
