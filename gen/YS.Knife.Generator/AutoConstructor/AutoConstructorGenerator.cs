using System;
using System.Collections.Concurrent;
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
        static AutoConstructorGenerator()
        {
            AutoConstructorGenerator.AddExtensionField(new ListFieldAttribute());
        }
        private static IDictionary<Type, AutoConstructorExtensionFieldAttribute> ExtensionFields = new  ConcurrentDictionary<Type, AutoConstructorExtensionFieldAttribute>();

        public static void AddExtensionField(AutoConstructorExtensionFieldAttribute fieldAttribute)
        {
            if (fieldAttribute != null)
            {
                ExtensionFields[fieldAttribute.GetType()] = fieldAttribute;
            }
           
        }

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
            var nameMapper = GetSymbolNameMapper(codeWriter.Compilation, classSymbol);
            if (!nameMapper.Any())
            {
                return null;
            }
            CsharpCodeBuilder builder = new CsharpCodeBuilder();
            AppendNamespace(classSymbol, builder);
            AppendClassDefinition(classSymbol, builder);
            AppendExtensionFields(classSymbol, nameMapper, builder);
            AppendPublicCtor(classSymbol, nameMapper, builder);

            builder.EndAllSegments();
            return new CodeFile
            {
                BasicName = string.Join(".", classSymbol.GetContainerClassChains().Select(p => p.Name)),
                Content = builder.ToString(),
            };

        }

        private void AppendExtensionFields(INamedTypeSymbol _, IDictionary<string, ArgumentInfo> nameMapper, CsharpCodeBuilder builder)
        {
            foreach (var newField in nameMapper.Values.Where(p => p.Source == ArgumentSource.NewField))
            {

                builder.AppendCodeLines($"private readonly {newField.MemberTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} {newField.MemberName};");
            }
        }

        IDictionary<string, ArgumentInfo> GetSymbolNameMapper(Compilation compilation, INamedTypeSymbol classSymbol)
        {
            var nameMapper = new Dictionary<string, ArgumentInfo>();


            foreach (var baseParam in GetBaseTypeParameters())
            {

                var newName = NewArgumentName(baseParam.Name.ToCamelCase(), nameMapper);
                nameMapper[newName] = new ArgumentInfo
                {
                    ArgName = newName,
                    ArgTypeSymbol = baseParam.Type,
                    MemberName = baseParam.Name,
                    MemberTypeSymbol = baseParam.Type,
                    Source = ArgumentSource.BaseCtor
                };
            }
            foreach (var field in GetInstanceFields())
            {
                var newName = NewArgumentName(field.Name.ToCamelCase(), nameMapper);
                nameMapper[newName] = new ArgumentInfo
                {
                    ArgName = newName,
                    ArgTypeSymbol = field.Type,
                    MemberName = field.Name,
                    MemberTypeSymbol = field.Type,
                    Source = ArgumentSource.Field
                };
            }
            foreach (var property in GetInstanceProperties())
            {
                var newName = NewArgumentName(property.Name.ToCamelCase(), nameMapper);
                nameMapper[newName] = new ArgumentInfo
                {
                    ArgName = newName,
                    ArgTypeSymbol = property.Type,
                    MemberName = property.Name,
                    MemberTypeSymbol = property.Type,
                    Source = ArgumentSource.Property
                };
            }
            foreach (var (memberName, ctorType, memberType) in GetExtensionFields())
            {
                var newName = NewArgumentName(memberName, nameMapper);

                var ctorTypeSymbol = compilation.GetTypeByMetadataName(ctorType);
                var memberTypeSymbol = compilation.GetTypeByMetadataName(memberType);
                // TODO check type symbol is null

                // now, only support one generic type argument
                if (ctorTypeSymbol.IsGenericType)
                {
                    ctorTypeSymbol = ctorTypeSymbol.Construct(classSymbol);
                }
                if (memberTypeSymbol.IsGenericType)
                {
                    memberTypeSymbol = memberTypeSymbol.Construct(classSymbol);
                }
                nameMapper[newName] = new ArgumentInfo
                {
                    ArgName = newName,
                    ArgTypeSymbol = ctorTypeSymbol,
                    MemberName = memberName,
                    MemberTypeSymbol = memberTypeSymbol,
                    Source = ArgumentSource.NewField
                };

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
                    .Where(p => !p.IsStatic && !p.IsIndexer && p.CanBeReferencedByName && p.IsAutoProperty() && !p.HasAttribute(typeof(AutoConstructorIgnoreAttribute)));
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

            IEnumerable<(string MemberName, string CtorArgType,string MemberType)> GetExtensionFields()
            {
                foreach (var attr in classSymbol.GetAttributes())
                {
                    foreach (var kv in ExtensionFields)
                    {
                        if (attr.AttributeClass.SafeEquals(kv.Key))
                        {
                            yield return (kv.Value.Name, kv.Value.CtorArgType, kv.Value.FieldType);
                        }
                    }
                
                }
            }
            string NewArgumentName(string baseName, IDictionary<string, ArgumentInfo> ctx)
            {
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
        void AppendPublicCtor(INamedTypeSymbol classSymbol, IDictionary<string, ArgumentInfo> nameMapper, CsharpCodeBuilder codeBuilder)
        {

            string args = string.Join(", ", nameMapper.Select(p => p.Value.GetCtorMethodArgumentItem()));

            codeBuilder.AppendCodeLines($"public {classSymbol.Name}({args})");
            var baseCtorArgs = nameMapper.Values.Where(p => p.Source == ArgumentSource.BaseCtor);
            if (baseCtorArgs.Any())
            {
                string baseArgs = string.Join(", ", baseCtorArgs.Select(p => $"{p.MemberName}: {p.ArgName}"));
                codeBuilder.AppendCodeLines($"    : base({baseArgs})");
            }


            codeBuilder.BeginSegment();
            // lines
            foreach (var member in nameMapper.Values.Where(p => p.Source != ArgumentSource.BaseCtor))
            {
                codeBuilder.AppendCodeLines($"this.{member.MemberName} = {member.ArgName};");
            }

            codeBuilder.EndSegment();

        }
        private class ArgumentInfo
        {
            public string ArgName { get; set; }
            public ITypeSymbol ArgTypeSymbol { get; set; }

            public string MemberName { get; set; }

            public ITypeSymbol MemberTypeSymbol { get; set; }

            public ArgumentSource Source { get; set; }

            public string GetCtorMethodArgumentItem()
            {
                return $"{ArgTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)} {ArgName}";
            }
        }
        private enum ArgumentSource
        {
            BaseCtor,
            Field,
            Property,
            NewField
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
