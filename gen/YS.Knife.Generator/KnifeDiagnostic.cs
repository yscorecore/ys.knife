using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace YS.Knife
{
    static class KnifeDiagnostic
    {
        const string AssemblyName = "YS.Knife.Generator";
        public static Diagnostic NotRefKnifeGeneratorAssembly()
        {
            DiagnosticDescriptor desc = new DiagnosticDescriptor("Knife001", "YS.Knife", $"Assembly '{AssemblyName}' is not refrenced.", "Error", DiagnosticSeverity.Error, true);
            return Diagnostic.Create(desc, null);
        }
        public static class Singleton
        {
            public static Diagnostic AlreadyExistsConstructor(INamedTypeSymbol classType)
            {
                DiagnosticDescriptor desc = new DiagnosticDescriptor("KF0001", typeof(SingletonPatternAttribute).FullName, $"The type '{classType.ToDisplayString()}' has already exists constructor, can not use '{typeof(SingletonPatternAttribute).FullName}'.", "Error", DiagnosticSeverity.Error, true);
                return Diagnostic.Create(desc, null);
            }
        }
    }
}
