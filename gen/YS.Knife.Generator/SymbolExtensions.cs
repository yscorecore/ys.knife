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
            return HasAttribute(symbol,attributeType.FullName);
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

        public static IList<IFieldSymbol> GetAllInstanceFields(Compilation compilation,IEnumerable<FieldDeclarationSyntax> fieldDeclarationSyntaxes,Func<IFieldSymbol,bool> where)
        {
            List<IFieldSymbol> fieldSymbols = new List<IFieldSymbol>();
            foreach (FieldDeclarationSyntax field in fieldDeclarationSyntaxes)
            {
                SemanticModel model = compilation.GetSemanticModel(field.SyntaxTree);
                foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
                {
                    // Get the symbol being decleared by the field, and keep it if its annotated
                    IFieldSymbol fieldSymbol = model.GetDeclaredSymbol(variable) as IFieldSymbol;
                    if (fieldSymbol.CanBeReferencedByName && !fieldSymbol.IsStatic && where(fieldSymbol))
                    {
                        fieldSymbols.Add(fieldSymbol);
                    }
                }
            }
            return fieldSymbols;
        }
        public static IList<IFieldSymbol> GetAllInstanceFieldsByAttributeName(Compilation compilation, IEnumerable<FieldDeclarationSyntax> fieldDeclarationSyntaxes, string attributeMetaName)
        {
            return GetAllInstanceFields(compilation, fieldDeclarationSyntaxes, (p) => p.HasAttribute(attributeMetaName));
        }
    }
}
