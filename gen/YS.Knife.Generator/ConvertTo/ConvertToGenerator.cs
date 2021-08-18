using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YS.Knife
{
    [Generator]
    class ConvertToGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ConvertToSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is ConvertToSyntaxReceiver receiver))
                return;
            var codeWriter = new CodeWriter(context);

            codeWriter.ForeachClassSyntax(receiver.CandidateClasses, ProcessClass);
        }

        private CodeFile ProcessClass(INamedTypeSymbol classSymbol, CodeWriter codeWriter)
        {
            if (!classSymbol.HasAttribute(typeof(ConvertToAttribute)))
            {
                return null;
            }

            CsharpCodeBuilder codeBuilder = new CsharpCodeBuilder();
            AppendUsingLines(classSymbol, codeBuilder);
            AppendNamespace(classSymbol, codeBuilder);
            AppendClassDefinition(classSymbol, codeBuilder);

            foreach (var convertToAttr in classSymbol.GetAttributes())
            {
                if (convertToAttr.AttributeClass.SafeEquals(typeof(ConvertToAttribute)))
                {
                    AppendConvertToFunctions(convertToAttr, codeBuilder);
                }
            }

            codeBuilder.EndAllSegments();
            return new CodeFile
            {
                BasicName = classSymbol.GetCodeFileBasicName(),
                Content = codeBuilder.ToString(),
            };


        }
        void AppendUsingLines(INamedTypeSymbol _, CsharpCodeBuilder codeBuilder)
        {
            codeBuilder.AppendCodeLines("using System.Collections.Generic;");
            codeBuilder.AppendCodeLines("using System.Linq;");
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
            for (int i = 0; i < classSymbols.Count; i++)
            {
                var parentClass = classSymbols[i];
                if (i == classSymbols.Count - 1)
                {
                    codeBuilder.AppendCodeLines($@"static partial class {parentClass.GetClassSymbolDisplayText()}");
                }
                else
                {
                    codeBuilder.AppendCodeLines($@"partial class {parentClass.GetClassSymbolDisplayText()}");
                }
                codeBuilder.BeginSegment();
            }
        }

        void AppendConvertToFunctions(AttributeData attributeData, CsharpCodeBuilder codeBuilder)
        {
            var arguments = attributeData.ConstructorArguments;
            var fromType = arguments.First().Value as INamedTypeSymbol;
            var toType = arguments.Last().Value as INamedTypeSymbol;
            var methodName = $"To{toType.Name}";
            var toTypeDisplay = toType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var fromTypeDisplay = fromType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            addToMethodForSingle();
            addCopyToMethodForSingle();
            addToMethodForEnumable();
            addToMethodForQueryable();
            void addToMethodForSingle()
            {
                codeBuilder.AppendCodeLines($"public static {toTypeDisplay} {methodName}(this {fromTypeDisplay} source)");
                codeBuilder.BeginSegment();
                if (!fromType.IsValueType)
                {
                    codeBuilder.AppendCodeLines("if (source == null) return default;");
                }
                codeBuilder.AppendCodeLines($"return new {toTypeDisplay}");
                codeBuilder.BeginSegment();
                appendPropertyAssign(fromType, toType, (sourceName, targetName) => $"{targetName} = source.{sourceName},");
                codeBuilder.EndSegment("};");
                codeBuilder.EndSegment();
            }
            void addCopyToMethodForSingle()
            {
                codeBuilder.AppendCodeLines($"public static void {methodName}(this {fromTypeDisplay} source, {toTypeDisplay} target)");
                codeBuilder.BeginSegment();
                if (!fromType.IsValueType)
                {
                    codeBuilder.AppendCodeLines("if (source == null) return;");
                }
                if (!toType.IsValueType)
                {
                    codeBuilder.AppendCodeLines("if (target == null) return;");
                }
                appendPropertyAssign(fromType, toType, (sourceName, targetName) => $"target.{targetName} = source.{sourceName};");
                codeBuilder.EndSegment();
            }
            void addToMethodForEnumable()
            {
                codeBuilder.AppendCodeLines($"public static IEnumerable<{toTypeDisplay}> {methodName}(this IEnumerable<{fromTypeDisplay}> source)");
                codeBuilder.BeginSegment();
                codeBuilder.AppendCodeLines($"return source?.Select(p => new {toTypeDisplay}");
                codeBuilder.BeginSegment();
                appendPropertyAssign(fromType, toType, (sourceName, targetName) => $"{targetName} = p.{sourceName},");
                codeBuilder.EndSegment("});");
                codeBuilder.EndSegment();
            }
            void addToMethodForQueryable()
            {
                codeBuilder.AppendCodeLines($"public static IQueryable<{toTypeDisplay}> {methodName}(this IQueryable<{fromTypeDisplay}> source)");
                codeBuilder.BeginSegment();
                codeBuilder.AppendCodeLines($"return source?.Select(p => new {toTypeDisplay}");
                codeBuilder.BeginSegment();
                appendPropertyAssign(fromType, toType, (sourceName, targetName) => $"{targetName} = p.{sourceName},");
                codeBuilder.EndSegment("});");
                codeBuilder.EndSegment();
            }
            bool canAssign(INamedTypeSymbol source, INamedTypeSymbol target)
            {
                return true;
            }
            void appendPropertyAssign(INamedTypeSymbol sourceType, INamedTypeSymbol targetType, Func<string, string, string> lineBuilder)
            {
                var targetProps = targetType.GetMembers()
                     .OfType<IPropertySymbol>()
                     .Where(p => !p.IsReadOnly && p.CanBeReferencedByName && !p.IsStatic && !p.IsIndexer)
                     .Select(p => new { p.Name, Type = p.Type as INamedTypeSymbol })
                     .ToDictionary(p => p.Name, p => p.Type);
                var sourceProps = sourceType.GetMembers()
                    .OfType<IPropertySymbol>()
                    .Where(p => p.CanBeReferencedByName && !p.IsStatic && !p.IsIndexer && !p.IsWriteOnly)
                    .Select(p => new { p.Name, Type = p.Type as INamedTypeSymbol })
                    .ToDictionary(p => p.Name, p => p.Type);
                foreach (var prop in targetProps)
                {
                    if (sourceProps.TryGetValue(prop.Key, out var sourcePropType) && canAssign(sourcePropType, prop.Value))
                    {
                        codeBuilder.AppendCodeLines(lineBuilder(prop.Key, prop.Key));
                    }
                }
            }

            /**
                     public static IEnumerable<UserInfo> ToUserInfo(this IEnumerable<TUser> user)
        {
            return user?.Select(p => new UserInfo
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age,
            });
        }
        public static IQueryable<UserInfo> ToUserInfo(this IQueryable<TUser> user)
        {
            return user?.Select(p => new UserInfo
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age
            });
        }

        public static void ToUserInfo(this TUser source, UserInfo target)
        {

            if (userInfo != null && user != null)
            {
                userInfo.Id = user.Id;
                userInfo.Age = user.Age;
                userInfo.Name = user.Name;
            }
        }
             */
        }

        private class ConvertToSyntaxReceiver : ISyntaxReceiver
        {
            public IList<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
                    classDeclarationSyntax.AttributeLists.Any())
                {
                    CandidateClasses.Add(classDeclarationSyntax);
                }
            }
        }
    }
}
