﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
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
                    AppendConvertToFunctions(convertToAttr, codeBuilder, codeWriter);
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
        void AppendConvertToFunctions(ConvertContext context, ConvertMappingInfo mappingInfo, CsharpCodeBuilder codeBuilder)
        {
            var toType = mappingInfo.TargetType;
            var fromType = mappingInfo.SourceType;
            var toTypeDisplay = mappingInfo.TargetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var fromTypeDisplay = mappingInfo.SourceType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            AddToMethodForSingle();
            AddCopyToMethodForSingle();
            AddToMethodForEnumable();
            AddToMethodForQueryable();
            void AddToMethodForSingle()
            {
                codeBuilder.AppendCodeLines($"public static {toTypeDisplay} {mappingInfo.ConvertToMethodName}(this {fromTypeDisplay} source)");
                codeBuilder.BeginSegment();
                if (!fromType.IsValueType)
                {
                    codeBuilder.AppendCodeLines("if (source == null) return default;");
                }
                codeBuilder.AppendCodeLines($"return new {toTypeDisplay}");
                codeBuilder.BeginSegment();
                AppendPropertyAssign(new WalkedPaths(mappingInfo.TargetType), fromType, toType, "source", null, ",");
                codeBuilder.EndSegment("};");
                codeBuilder.EndSegment();
            }
            void AddCopyToMethodForSingle()
            {
                codeBuilder.AppendCodeLines($"public static void {mappingInfo.ConvertToMethodName}(this {fromTypeDisplay} source, {toTypeDisplay} target)");
                codeBuilder.BeginSegment();
                if (!fromType.IsValueType)
                {
                    codeBuilder.AppendCodeLines("if (source == null) return;");
                }
                if (!toType.IsValueType)
                {
                    codeBuilder.AppendCodeLines("if (target == null) return;");
                }
                AppendPropertyAssign(new WalkedPaths(mappingInfo.TargetType), fromType, toType, "source", "target", ";");
                codeBuilder.EndSegment();
            }
            void AddToMethodForEnumable()
            {
                codeBuilder.AppendCodeLines($"public static IEnumerable<{toTypeDisplay}> {mappingInfo.ConvertToMethodName}(this IEnumerable<{fromTypeDisplay}> source)");
                codeBuilder.BeginSegment();
                codeBuilder.AppendCodeLines($"return source?.Select(p => new {toTypeDisplay}");
                codeBuilder.BeginSegment();
                AppendPropertyAssign(new WalkedPaths(mappingInfo.TargetType), fromType, toType, "p", null, ",");
                codeBuilder.EndSegment("});");
                codeBuilder.EndSegment();
            }
            void AddToMethodForQueryable()
            {
                codeBuilder.AppendCodeLines($"public static IQueryable<{toTypeDisplay}> {mappingInfo.ConvertToMethodName}(this IQueryable<{fromTypeDisplay}> source)");
                codeBuilder.BeginSegment();
                codeBuilder.AppendCodeLines($"return source?.Select(p => new {toTypeDisplay}");
                codeBuilder.BeginSegment();
                AppendPropertyAssign(new WalkedPaths(mappingInfo.TargetType), fromType, toType, "p", null, ",");
                codeBuilder.EndSegment("});");
                codeBuilder.EndSegment();
            }
            bool CanAssign(INamedTypeSymbol source, INamedTypeSymbol target)
            {
                var conversion = context.CodeWriter.Compilation.ClassifyConversion(source, target);
                return conversion.IsImplicit || conversion.IsReference || conversion.IsNullable || conversion.IsBoxing;
            }
            string FormatRefrence(string refrenceName, string expression)
            {
                if (string.IsNullOrEmpty(refrenceName))
                {
                    return expression;
                }
                return $"{refrenceName}.{expression}";
            }
            void AppendPropertyAssign(WalkedPaths walkedPaths, INamedTypeSymbol sourceType, INamedTypeSymbol targetType, string sourceRefrenceName, string targetRefrenceName, string lineSplitChar)
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
                    if (mappingInfo.IgnoreTargetProperties != null && mappingInfo.IgnoreTargetProperties.Contains(prop.Key))
                    {
                        continue;
                    }
                    if (mappingInfo.CustomerMappings != null && mappingInfo.CustomerMappings.TryGetValue(prop.Key, out var sourceExpression))
                    {
                        var actualSourceExpression = sourceExpression.Replace("$", sourceRefrenceName);
                        codeBuilder.AppendCodeLines($"{FormatRefrence(targetRefrenceName, prop.Key)} = {actualSourceExpression}{lineSplitChar}");
                    }
                    else if (sourceProps.TryGetValue(prop.Key, out var sourcePropType))
                    {
                        if (CanAssign(sourcePropType, prop.Value))
                        {
                            // default 
                            codeBuilder.AppendCodeLines($"{FormatRefrence(targetRefrenceName, prop.Key)} = {FormatRefrence(sourceRefrenceName, prop.Key)}{lineSplitChar}");
                        }
                        else
                        {
                            // object
                            if (CanMappingSubObject(sourcePropType, prop.Value) && !walkedPaths.HasWalked(prop.Value))
                            {

                                MappingSubObjectProperty(walkedPaths.Fork(prop.Value), sourcePropType, prop.Value, sourceRefrenceName, targetRefrenceName, prop.Key, lineSplitChar);
                            }

                        }


                    }
                }

            }
            bool CanMappingSubObject(INamedTypeSymbol sourceType, INamedTypeSymbol targetType)
            {
                return targetType.TypeKind == TypeKind.Class && !targetType.IsAbstract && targetType.HasEmptyCtor();
            }
            void MappingSubObjectProperty(WalkedPaths walkedPaths, INamedTypeSymbol sourcePropertyType, INamedTypeSymbol targetPropertyType, string sourceRefrenceName, string targetRefrenceName, string propertyName, string lineSplitChar)
            {
                var targetPropertyTypeText = targetPropertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var targetPropertyExpression = FormatRefrence(targetRefrenceName, propertyName);
                var sourcePropertyExpression = FormatRefrence(sourceRefrenceName, propertyName);
                if (sourcePropertyType.IsValueType)
                {

                    codeBuilder.AppendCodeLines($"{targetPropertyExpression} = new {targetPropertyTypeText}");
                    codeBuilder.BeginSegment();
                    AppendPropertyAssign(walkedPaths, sourcePropertyType, targetPropertyType, sourcePropertyExpression, null, ",");
                    codeBuilder.EndSegment("}" + lineSplitChar);
                }
                else
                {
                    codeBuilder.AppendCodeLines($"{targetPropertyExpression} = {sourcePropertyExpression} == null ? default({targetPropertyTypeText}): new {targetPropertyTypeText}");
                    codeBuilder.BeginSegment();
                    AppendPropertyAssign(walkedPaths, sourcePropertyType, targetPropertyType, sourcePropertyExpression, null, ",");
                    codeBuilder.EndSegment("}" + lineSplitChar);
                }
            }
        }
        void AppendConvertToFunctions(AttributeData attributeData, CsharpCodeBuilder codeBuilder, CodeWriter codeWriter)
        {
            var context = new ConvertContext(codeWriter);
            AppendConvertToFunctions(context, ConvertMappingInfo.FromAttributeData(attributeData), codeBuilder);
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
        private class ConvertContext
        {
            public ConvertContext(CodeWriter codeWriter)
            {
                CodeWriter = codeWriter;
            }

            public CodeWriter CodeWriter { get; }

        }
        private class WalkedPaths
        {
            public WalkedPaths(params INamedTypeSymbol[] paths)
            {
                this.Paths.AddRange(paths);
            }
            private List<INamedTypeSymbol> Paths { get; } = new List<INamedTypeSymbol>();

            public WalkedPaths Fork(INamedTypeSymbol symbol)
            {
                var paths = Paths.Concat(new INamedTypeSymbol[] { symbol });
                return new WalkedPaths(paths.ToArray());
            }
            public bool HasWalked(INamedTypeSymbol symbol)
            {
                foreach (var path in Paths)
                {
                    if (path.Equals(symbol, SymbolEqualityComparer.Default))
                    {
                        return true;
                    }
                }
                return false;
            }

        }

        private class ConvertMappingInfo
        {
            public INamedTypeSymbol TargetType { get; private set; }
            public INamedTypeSymbol SourceType { get; private set; }
            public HashSet<string> IgnoreTargetProperties { get; private set; }
            public Dictionary<string, string> CustomerMappings { get; private set; }
            public string ConvertToMethodName { get; private set; }

            public static ConvertMappingInfo FromAttributeData(AttributeData attributeData)
            {
                var arguments = attributeData.ConstructorArguments;
                var fromType = arguments.First().Value as INamedTypeSymbol;
                var toType = arguments.Last().Value as INamedTypeSymbol;
                var ignoreProperties = attributeData.NamedArguments
                    .Where(p => p.Key == nameof(ConvertToAttribute.IgnoreTargetProperties))
                    .Where(p => p.Value.IsNull == false)
                    .SelectMany(p => p.Value.Values.Select(t => (string)t.Value))
                    .Where(p => !string.IsNullOrWhiteSpace(p));
                var customMappings = attributeData.NamedArguments
                    .Where(p => p.Key == nameof(ConvertToAttribute.CustomMappings))
                    .Where(p => p.Value.IsNull == false)
                    .SelectMany(p => p.Value.Values.Select(t => (string)t.Value))
                    .Select(ParseCustomMapping)
                    .Where(item => item != null)
                    .Select(item => item.Value)
                    .ToLookup(item => item.Key, item => item.Value)
                    .ToDictionary(p => p.Key, p => p.Last());
                return new ConvertMappingInfo
                {
                    SourceType = fromType,
                    TargetType = toType,
                    IgnoreTargetProperties = new HashSet<string>(ignoreProperties),
                    CustomerMappings = customMappings,
                    ConvertToMethodName = $"To{toType.Name}",
                };
            }

            public static ConvertMappingInfo Create(INamedTypeSymbol source, INamedTypeSymbol target, INamedTypeSymbol hostClasses)
            {

                var attributeData = hostClasses.GetAttributes()
                     .Where(p => p.AttributeClass.SafeEquals(typeof(ConvertToAttribute)))
                     .Where(p => (p.ConstructorArguments.First().Value as INamedTypeSymbol).Equals(source, SymbolEqualityComparer.Default))
                     .Where(p => (p.ConstructorArguments.Last().Value as INamedTypeSymbol).Equals(source, SymbolEqualityComparer.Default))
                     .FirstOrDefault();
                if (attributeData != null)
                {
                    return FromAttributeData(attributeData);
                }
                else
                {
                    return new ConvertMappingInfo
                    {
                        SourceType = source,
                        TargetType = target,
                        CustomerMappings = new Dictionary<string, string>(),
                        IgnoreTargetProperties = new HashSet<string>()
                    };
                }
            }

            static KeyValuePair<string, string>? ParseCustomMapping(string expression)
            {
                if (string.IsNullOrWhiteSpace(expression))
                {
                    return null;
                }
                var equalIndex = expression.IndexOf('=');
                if (equalIndex > 0)
                {
                    var targetExpression = expression.Substring(0, equalIndex).Trim();
                    var sourceExpression = expression.Substring(equalIndex + 1).Trim();
                    return new KeyValuePair<string, string>(targetExpression, sourceExpression);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
