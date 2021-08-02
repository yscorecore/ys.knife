﻿using System;
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

            return symbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == other?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }
        public static bool SafeEquals(this INamedTypeSymbol symbol, string typeMetaName)
        {
            return symbol?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == $"global::{typeMetaName}";
        }
        public static bool SafeEquals(this INamedTypeSymbol symbol, Type type)
        {
            return SafeEquals(symbol, type.FullName);
        }
        public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attributeSymbol)
        {
            return symbol.GetAttributes().Any(ad =>
                        ad.AttributeClass.SafeEquals(attributeSymbol));
        }
        public static bool HasAttribute(this ISymbol symbol, string attributeMetaType)
        {
            return symbol.GetAttributes().Any(ad =>
                        ad.AttributeClass.SafeEquals(attributeMetaType));
        }
        public static bool HasAttribute(this ISymbol symbol, Type attributeType)
        {
            return HasAttribute(symbol, attributeType.FullName);
        }

        public static IList<INamedTypeSymbol> GetParentClassChains(this INamedTypeSymbol classSymbol)
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

        public static IEnumerable<IFieldSymbol> GetAllInstanceFieldsByAttribute(this INamedTypeSymbol clazzSymbol, Type attributeType)
        {
            return clazzSymbol.GetMembers().OfType<IFieldSymbol>()
                .Where(p => p.CanBeReferencedByName && !p.IsStatic && p.HasAttribute(attributeType));

        }

        public static IEnumerable<INamedTypeSymbol> GetAllClassSymbolsIgnoreRepeated(this CodeWriter codeWriter, IEnumerable<ClassDeclarationSyntax> classDeclarationSyntax)
        {
            var classSymbols = new HashSet<string>();
            foreach (var clazz in classDeclarationSyntax)
            {
                SemanticModel model = codeWriter.Compilation.GetSemanticModel(clazz.SyntaxTree);
                var clazzSymbol = model.GetDeclaredSymbol(clazz) as INamedTypeSymbol;

                var clazzSymbolAualifiedName = clazzSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                if (!classSymbols.Contains(clazzSymbolAualifiedName))
                {
                    classSymbols.Add(clazzSymbolAualifiedName);
                    yield return clazzSymbol;
                }
            }
        }
    }
}
