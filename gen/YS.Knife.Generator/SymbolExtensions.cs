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
        public static bool HasAttribute(this ISymbol symbol, INamedTypeSymbol attributeSymbol)
        {
            return symbol.GetAttributes().Any(ad =>
                        ad.AttributeClass.SafeEquals(attributeSymbol));
        }

        public static IEnumerable<ClassDeclarationSyntax> DistinctClasssSyntax(this IEnumerable<ClassDeclarationSyntax> sources )
        {
            return sources.Distinct(ClassDeclarationSyntaxComparer.Instance);
        }

        class ClassDeclarationSyntaxComparer : IEqualityComparer<ClassDeclarationSyntax>
        {
            public static ClassDeclarationSyntaxComparer Instance = new ClassDeclarationSyntaxComparer();

            public bool Equals(ClassDeclarationSyntax x, ClassDeclarationSyntax y)
            {
             return  x.ToFullString() == y.ToFullString(); 
            }

            public int GetHashCode(ClassDeclarationSyntax obj)
            {
                return obj.ToFullString().GetHashCode();
            }
        }
    }
}
