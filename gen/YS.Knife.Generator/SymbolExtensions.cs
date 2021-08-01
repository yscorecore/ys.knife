using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

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
    }
}
