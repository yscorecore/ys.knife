using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YS.Knife
{
    static class SymbolExtensions
    {
        public static bool SafeEquals(this ISymbol symbol, ISymbol other)
        {
            if (symbol.Equals(other, SymbolEqualityComparer.Default))
            {
                return true;
            }

            return symbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ==
                   other?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        private static bool SafeEquals(this INamedTypeSymbol symbol, string globalTypeMetaName)
        {
            return symbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == globalTypeMetaName;
        }

        private static string TypeNameCombinGlobal(this string typeName) => $"global::{typeName}";
        private static string GetGlobalTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var index = type.FullName.IndexOf('`');
                string typeName = type.FullName.Substring(0, index);
                if (type.IsGenericTypeDefinition)
                {
                    var argumentLength = type.GetGenericArguments().Length;
                    return $"{typeName.TypeNameCombinGlobal()}<{new string(',', argumentLength - 1)}>";
                }
                else
                {
                    var arguments = type.GetGenericArguments().Select(p => GetGlobalTypeName(p));
                    return $"{typeName.TypeNameCombinGlobal()}<{string.Join(",", arguments)}>";
                }
            }
            else
            {
                return type.FullName.TypeNameCombinGlobal();
            }
        }
        public static bool SafeEquals(this INamedTypeSymbol symbol, Type type)
        {
            return SafeEquals(symbol, GetGlobalTypeName(type));
        }

        public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attributeSymbol)
        {
            return symbol.GetAttributes().Any(ad =>
                ad.AttributeClass.SafeEquals(attributeSymbol));
        }

        public static bool HasAttribute(this ISymbol symbol, string attributeMetaType)
        {
            return symbol.GetAttributes().Any(ad =>
                ad.AttributeClass.SafeEquals(attributeMetaType.TypeNameCombinGlobal()));
        }

        public static bool HasAttribute(this ISymbol symbol, Type attributeType)
        {
            return HasAttribute(symbol, attributeType.FullName);
        }

        public static string GetClassSymbolDisplayText(this INamedTypeSymbol classSymbol)
        {
            if (classSymbol.TypeArguments.Length > 0)
            {
                return $"{classSymbol.Name}<{string.Join(", ", classSymbol.TypeArguments.Select(p => p.Name))}>";
            }
            else
            {
                return classSymbol.Name;
            }
        }
        public static bool IsAutoProperty(this IPropertySymbol propertySymbol)
        {
            var fields = propertySymbol.ContainingType.GetMembers().OfType<IFieldSymbol>();

            return fields.Any(field => !field.CanBeReferencedByName && SymbolEqualityComparer.Default.Equals(field.AssociatedSymbol, propertySymbol));
        }
        public static IList<INamedTypeSymbol> GetContainerClassChains(this INamedTypeSymbol classSymbol)
        {
            var namespaceSymbol = classSymbol.ContainingNamespace;
            List<INamedTypeSymbol> paths = new List<INamedTypeSymbol>();
            while (classSymbol != null)
            {
                paths.Insert(0, classSymbol);
                if (classSymbol.ContainingSymbol.Equals(namespaceSymbol, SymbolEqualityComparer.Default))
                {
                    break;
                }
                else
                {
                    classSymbol = classSymbol.ContainingSymbol as INamedTypeSymbol;
                }
            }

            return paths.AsReadOnly();
        }

        public static IEnumerable<IFieldSymbol> GetAllInstanceFieldsByAttribute(this INamedTypeSymbol clazzSymbol,
            Type attributeType)
        {
            return clazzSymbol.GetMembers().OfType<IFieldSymbol>()
                .Where(p => p.CanBeReferencedByName && !p.IsStatic && p.HasAttribute(attributeType));
        }

        public static IEnumerable<INamedTypeSymbol> GetAllClassSymbolsIgnoreRepeated(this CodeWriter codeWriter,
            IEnumerable<ClassDeclarationSyntax> classDeclarationSyntax)
        {
            var classSymbols = new HashSet<string>();
            foreach (var clazz in classDeclarationSyntax)
            {
                SemanticModel model = codeWriter.Compilation.GetSemanticModel(clazz.SyntaxTree);
                var clazzSymbol = model.GetDeclaredSymbol(clazz) as INamedTypeSymbol;
                if (classSymbols.Contains(clazzSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)))
                {
                    continue;
                }

                foreach (var dependency in GetDependencyTree(clazzSymbol))
                {
                    var clazzSymbolAualifiedName = dependency.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    if (!classSymbols.Contains(clazzSymbolAualifiedName))
                    {
                        classSymbols.Add(clazzSymbolAualifiedName);
                        yield return dependency;
                    }

                }
            }

            IEnumerable<INamedTypeSymbol> GetDependencyTree(INamedTypeSymbol classSymbol)
            {
                List<INamedTypeSymbol> result = new List<INamedTypeSymbol>();
                result.Add(classSymbol);

                var assembly = classSymbol.ContainingAssembly;



                if (assembly != null)
                {
                    var current = classSymbol;
                    while (current.BaseType != null && assembly.Equals(current.BaseType.ContainingAssembly, SymbolEqualityComparer.Default))
                    {
                        result.Insert(0, current.BaseType);
                        current = current.BaseType;
                    }

                }


                return result;
            }
        }

        public static string GetCodeFileBasicName(this INamedTypeSymbol classSymbol)
        {
            return string.Join(".", classSymbol.GetContainerClassChains().Select(p => p.Name));
        }
        public static bool HasEmptyCtor(this INamedTypeSymbol classSymbol)
        {
            return classSymbol.InstanceConstructors.Any(p => p.Parameters.Count() == 0);
        }
        public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type)
        {
            var current = type;
            while (current != null)
            {
                foreach (var member in current.GetMembers())
                {
                    yield return member;
                }
                current = current.BaseType;
            }

        }
        public static bool IsPrimitive(this ITypeSymbol type)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));


            switch (type.SpecialType)
            {
                case SpecialType.System_Boolean:
                case SpecialType.System_SByte:
                case SpecialType.System_Int16:
                case SpecialType.System_Int32:
                case SpecialType.System_Int64:
                case SpecialType.System_Byte:
                case SpecialType.System_UInt16:
                case SpecialType.System_UInt32:
                case SpecialType.System_UInt64:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_Char:
                case SpecialType.System_String:
                case SpecialType.System_Object:
                    return true;
            }
            return false;
        }
    }
}
