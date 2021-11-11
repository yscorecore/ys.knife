using System;
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
                if (!convertToAttr.AttributeClass.SafeEquals(typeof(ConvertToAttribute)))
                {
                    continue;
                }
                var convertMappingInfo = ConvertMappingInfo.FromAttributeData(convertToAttr);
                if (ConvertMappingInfo.CanMappingSubObject(convertMappingInfo.SourceType, convertMappingInfo.TargetType))
                {
                    AppendConvertToFunctions(new ConvertContext(
                          classSymbol,
                          codeWriter.Compilation,
                          codeBuilder, convertMappingInfo));
                }
                else
                {

                    // TOTO report error
                }

            }

            codeBuilder.EndAllSegments();
            return new CodeFile
            {
                BasicName = classSymbol.GetCodeFileBasicName(),
                Content = codeBuilder.ToString(),
            };


        }
        private void AppendUsingLines(INamedTypeSymbol _, CsharpCodeBuilder codeBuilder)
        {
            codeBuilder.AppendCodeLines("using System.Collections.Generic;");
            codeBuilder.AppendCodeLines("using System.Linq;");
        }
        private void AppendNamespace(INamedTypeSymbol classSymbol, CsharpCodeBuilder codeBuilder)
        {
            if (!classSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                codeBuilder.AppendCodeLines($"namespace {classSymbol.ContainingNamespace.ToDisplayString()}");
                codeBuilder.BeginSegment();
            }
        }
        private void AppendClassDefinition(INamedTypeSymbol classSymbol, CsharpCodeBuilder codeBuilder)
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
        private void AppendConvertToFunctions(ConvertContext context)
        {
            var mappingInfo = context.MappingInfo;
            var codeBuilder = context.CodeBuilder;
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
                AppendPropertyAssign("source", null, ",", context);
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
                AppendPropertyAssign("source", "target", ";", context);
                codeBuilder.EndSegment();
            }
            void AddToMethodForEnumable()
            {
                codeBuilder.AppendCodeLines($"public static IEnumerable<{toTypeDisplay}> {mappingInfo.ConvertToMethodName}(this IEnumerable<{fromTypeDisplay}> source)");
                codeBuilder.BeginSegment();
                codeBuilder.AppendCodeLines($"return source?.Select(p => new {toTypeDisplay}");
                codeBuilder.BeginSegment();
                AppendPropertyAssign("p", null, ",", context);
                codeBuilder.EndSegment("});");
                codeBuilder.EndSegment();
            }
            void AddToMethodForQueryable()
            {
                codeBuilder.AppendCodeLines($"public static IQueryable<{toTypeDisplay}> {mappingInfo.ConvertToMethodName}(this IQueryable<{fromTypeDisplay}> source)");
                codeBuilder.BeginSegment();
                codeBuilder.AppendCodeLines($"return source?.Select(p => new {toTypeDisplay}");
                codeBuilder.BeginSegment();
                AppendPropertyAssign("p", null, ",", context);
                codeBuilder.EndSegment("});");
                codeBuilder.EndSegment();
            }

        }
        private bool CanMappingSubObjectProperty(ITypeSymbol sourceType, ITypeSymbol targetType, ConvertContext convertContext)
        {
            if (sourceType.IsPrimitive() || targetType.IsPrimitive())
            {
                return false;
            }
            if (convertContext.HasWalked(targetType))
            {
                return false;
            }
            if (targetType is INamedTypeSymbol namedTargetType)
            {

                if (sourceType.TypeKind == TypeKind.Class || sourceType.TypeKind == TypeKind.Struct)
                {
                    if (targetType.TypeKind == TypeKind.Struct) return true;
                    return targetType.TypeKind == TypeKind.Class && !targetType.IsAbstract && namedTargetType.HasEmptyCtor();
                }
            }

            return false;
        }
        private bool CanMappingCollectionProperty(ITypeSymbol sourcePropType, ITypeSymbol targetPropType, ConvertContext convertContext)
        {
            if (SourceTypeIsEnumerable() && TargetTypeIsSupportedEnumerable())
            {
                var sourceItemType = GetItemType(sourcePropType);
                var targetItemType = GetItemType(targetPropType);
                return CanAssign(sourceItemType, targetItemType, convertContext) || CanMappingSubObjectProperty(sourceItemType, targetItemType, convertContext);
            }
            return false;

            bool SourceTypeIsEnumerable()
            {
                if (sourcePropType is IArrayTypeSymbol)
                {
                    return true;
                }
                if (sourcePropType is INamedTypeSymbol namedSourcePropType)
                {
                    if (namedSourcePropType.IsGenericType)
                    {
                        if (namedSourcePropType.ConstructUnboundGenericType().SafeEquals(typeof(IEnumerable<>)))
                        {
                            return true;
                        }
                        if (sourcePropType.AllInterfaces.Any(p => p.IsGenericType && p.ConstructUnboundGenericType().SafeEquals(typeof(IEnumerable<>))))
                        {
                            return true;
                        }
                    }
                }

                return false;

            }
            bool TargetTypeIsSupportedEnumerable()
            {

                if (targetPropType is IArrayTypeSymbol)
                {
                    return true;
                }
                if (targetPropType is INamedTypeSymbol namedTargetPropType)
                {
                    if (namedTargetPropType.IsGenericType)
                    {
                        var targetUnboundGenericType = namedTargetPropType.ConstructUnboundGenericType();
                        return
                           targetUnboundGenericType.SafeEquals(typeof(IList<>)) ||
                            targetUnboundGenericType.SafeEquals(typeof(List<>)) ||
                            targetUnboundGenericType.SafeEquals(typeof(IEnumerable<>)) ||
                            targetUnboundGenericType.SafeEquals(typeof(IQueryable<>)) ||
                            targetUnboundGenericType.SafeEquals(typeof(ICollection<>));
                    }
                }

                return false;
            }


        }
        private ITypeSymbol GetItemType(ITypeSymbol typeSymbol)
        {
            if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                return arrayTypeSymbol.ElementType;
            }
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
            {
                return namedTypeSymbol.TypeArguments[0];
            }
            return null;
        }
        private void MappingSubObjectProperty(ConvertContext convertContext, string sourceRefrenceName, string targetRefrenceName, string propertyName, string lineSplitChar)
        {
            var targetPropertyType = convertContext.MappingInfo.TargetType;
            var sourcePropertyType = convertContext.MappingInfo.SourceType;
            var codeBuilder = convertContext.CodeBuilder;
            var targetPropertyTypeText = targetPropertyType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            var targetPropertyExpression = FormatRefrence(targetRefrenceName, propertyName);
            var sourcePropertyExpression = FormatRefrence(sourceRefrenceName, propertyName);
            if (sourcePropertyType.IsValueType)
            {
                codeBuilder.AppendCodeLines($"{targetPropertyExpression} = new {targetPropertyTypeText}");
            }
            else
            {
                codeBuilder.AppendCodeLines($"{targetPropertyExpression} = {sourcePropertyExpression} == null ? default({targetPropertyTypeText}): new {targetPropertyTypeText}");
            }
            codeBuilder.BeginSegment();
            AppendPropertyAssign(sourcePropertyExpression, null, ",", convertContext);
            codeBuilder.EndSegment("}" + lineSplitChar);
        }

        private void MappingCollectionProperty(ConvertContext convertContext,
            string sourceRefrenceName, string targetRefrenceName, string propertyName, string lineSplitChar)
        {
            var codeBuilder = convertContext.CodeBuilder;
            var targetPropertyType = convertContext.MappingInfo.TargetType;
            var sourcePropertyType = convertContext.MappingInfo.SourceType;
            var targetItemType = GetItemType(targetPropertyType);
            var sourceItemType = GetItemType(sourcePropertyType);
            var targetItemTypeText = targetItemType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

            var targetPropertyExpression = FormatRefrence(targetRefrenceName, propertyName);
            var sourcePropertyExpression = FormatRefrence(sourceRefrenceName, propertyName);

            if (sourceItemType.SafeEquals(targetItemType))
            {

                codeBuilder.AppendCodeLines($"{targetPropertyExpression} = {sourcePropertyExpression} == null ? null : {sourcePropertyExpression}.{ToTargetMethodName()}(){lineSplitChar}");
            }
            else if (CanAssign(sourceItemType, targetItemType, convertContext))
            {
                codeBuilder.AppendCodeLines($"{targetPropertyExpression} = {sourcePropertyExpression} == null ? null : {sourcePropertyExpression}.Cast<{targetItemTypeText}>().{ToTargetMethodName()}(){lineSplitChar}");
            }
            else
            {
                if (sourceItemType.IsValueType)
                {
                    codeBuilder.AppendCodeLines($"{targetPropertyExpression} = {sourcePropertyExpression} == null ? null : {sourcePropertyExpression}.Select(p => new {targetItemTypeText}");
                }
                else
                {
                    codeBuilder.AppendCodeLines($"{targetPropertyExpression} = {sourcePropertyExpression} == null ? null : {sourcePropertyExpression}.Select(p => p == null ? default({targetItemTypeText}) : new {targetItemTypeText}");

                }
                codeBuilder.BeginSegment();
                var newConvertContext = convertContext.Fork(sourceItemType, targetItemType);
                AppendPropertyAssign("p", null, ",", newConvertContext);
                codeBuilder.EndSegment("})." + $"{ToTargetMethodName()}(){lineSplitChar}");

            }

            string ToTargetMethodName()
            {
                if (targetPropertyType is IArrayTypeSymbol arrayTypeSymbol)
                {
                    return nameof(Enumerable.ToArray);
                }

                if ((targetPropertyType as INamedTypeSymbol).ConstructUnboundGenericType()
                    .SafeEquals(typeof(IQueryable<>)))
                {
                    return nameof(Queryable.AsQueryable);
                }

                return nameof(Enumerable.ToList);
            }

        }

        private bool CanAssign(ITypeSymbol source, ITypeSymbol target, ConvertContext context)
        {
            var conversion = context.Compilation.ClassifyConversion(source, target);
            return conversion.IsImplicit || conversion.IsBoxing;
        }
        private string FormatRefrence(string refrenceName, string expression)
        {
            if (string.IsNullOrEmpty(refrenceName))
            {
                return expression;
            }
            return $"{refrenceName}.{expression}";
        }
        private void AppendPropertyAssign(string sourceRefrenceName, string targetRefrenceName, string lineSplitChar, ConvertContext convertContext)
        {
            var mappingInfo = convertContext.MappingInfo;
            var codeBuilder = convertContext.CodeBuilder;
            var targetProps = mappingInfo.TargetType.GetAllMembers()
                 .OfType<IPropertySymbol>()
                 .Where(p => !p.IsReadOnly && p.CanBeReferencedByName && !p.IsStatic && !p.IsIndexer)
                 .Select(p => new { p.Name, Type = p.Type })
                 .ToLookup(p=>p.Name)
                 .ToDictionary(p => p.Key, p => p.First().Type);
            var sourceProps = mappingInfo.SourceType.GetAllMembers()
                .OfType<IPropertySymbol>()
                .Where(p => p.CanBeReferencedByName && !p.IsStatic && !p.IsIndexer && !p.IsWriteOnly)
                .Select(p => new { p.Name, Type = p.Type })
                .ToLookup(p=>p.Name)
                .ToDictionary(p => p.Key, p => p.First().Type);
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
                    if (CanAssign(sourcePropType, prop.Value, convertContext))
                    {
                        // default 
                        codeBuilder.AppendCodeLines($"{FormatRefrence(targetRefrenceName, prop.Key)} = {FormatRefrence(sourceRefrenceName, prop.Key)}{lineSplitChar}");
                    }
                    else if (CanMappingCollectionProperty(sourcePropType, prop.Value, convertContext))
                    {
                        // collection
                        var newConvertContext = convertContext.Fork(sourcePropType, prop.Value);
                        MappingCollectionProperty(newConvertContext, sourceRefrenceName, targetRefrenceName, prop.Key, lineSplitChar);
                    }
                    else if (CanMappingSubObjectProperty(sourcePropType, prop.Value, convertContext))
                    {
                        // sub object 
                        var newConvertContext = convertContext.Fork(sourcePropType, prop.Value);
                        MappingSubObjectProperty(newConvertContext, sourceRefrenceName, targetRefrenceName, prop.Key, lineSplitChar);
                    }
                }
            }
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
            public ConvertContext(INamedTypeSymbol hostClass, Compilation compilation, CsharpCodeBuilder codeBuilder, ConvertMappingInfo convertMappingInfo)
            {
                this.HostClass = hostClass;
                this.Compilation = compilation;
                this.MappingInfo = convertMappingInfo;
                this.CodeBuilder = codeBuilder;
                this.WorkedPaths.Add(convertMappingInfo.TargetType);
            }
            private ConvertContext(ConvertContext baseConvertContext, ConvertMappingInfo convertMappingInfo)
                : this(baseConvertContext.HostClass, baseConvertContext.Compilation, baseConvertContext.CodeBuilder, convertMappingInfo)
            {
                this.WorkedPaths = new List<ITypeSymbol>(baseConvertContext.WorkedPaths);
                this.WorkedPaths.Add(convertMappingInfo.TargetType);
            }
            public Compilation Compilation { get; }
            public ConvertMappingInfo MappingInfo { get; }
            public CsharpCodeBuilder CodeBuilder { get; }
            public INamedTypeSymbol HostClass { get; }
            public List<ITypeSymbol> WorkedPaths { get; } = new List<ITypeSymbol>();
            public bool HasWalked(ITypeSymbol symbol)
            {
                foreach (var path in WorkedPaths)
                {
                    if (path.Equals(symbol, SymbolEqualityComparer.Default))
                    {
                        return true;
                    }
                }
                return false;
            }
            public ConvertContext Fork(ITypeSymbol source, ITypeSymbol target)
            {
                var newMappingInfo = ConvertMappingInfo.Create(source, target, this.HostClass);
                return new ConvertContext(this, newMappingInfo);
            }
        }
        private class ConvertMappingInfo
        {
            public ITypeSymbol TargetType { get; private set; }
            public ITypeSymbol SourceType { get; private set; }
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

            public static ConvertMappingInfo Create(ITypeSymbol source, ITypeSymbol target, INamedTypeSymbol hostClasses)
            {

                var attributeData = hostClasses.GetAttributes().Reverse()
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

            public static bool CanMappingSubObject(ITypeSymbol sourceType, ITypeSymbol targetType)
            {
                if (targetType is INamedTypeSymbol namedTargetType)
                {
                    if (sourceType.TypeKind == TypeKind.Class || sourceType.TypeKind == TypeKind.Struct)
                    {
                        if (targetType.TypeKind == TypeKind.Struct) return true;
                        return targetType.TypeKind == TypeKind.Class && !targetType.IsAbstract && namedTargetType.HasEmptyCtor();
                    }
                }

                return false;
            }
        }
    }
}
