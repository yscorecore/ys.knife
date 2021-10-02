using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace YS.Knife
{
    static class KnifeDiagnostic
    {

        public static class Singleton
        {
            public static Diagnostic AlreadyExistsConstructor(INamedTypeSymbol classType)
            {
                DiagnosticDescriptor desc = new DiagnosticDescriptor("KF0001", typeof(SingletonPatternAttribute).FullName, $"The type '{classType.ToDisplayString()}' has already exists constructor, can not use '{typeof(SingletonPatternAttribute).FullName}'.", "Error", DiagnosticSeverity.Error, true);
                return Diagnostic.Create(desc, null);
            }
        }
        public static class AutoNotify
        {
            public static Diagnostic InvalidPropertyName(string propName)
            {
                DiagnosticDescriptor desc = new DiagnosticDescriptor("KF1001", typeof(AutoNotifyAttribute).FullName, $"Invalid property name '{propName}'.", "Error", DiagnosticSeverity.Error, true);
                return Diagnostic.Create(desc, null);
            }
            public static Diagnostic PropertyNameEqualFieldName(string propName)
            {
                DiagnosticDescriptor desc = new DiagnosticDescriptor("KF1002", typeof(AutoNotifyAttribute).FullName, $"The property name '{propName}' and field name are equal.", "Error", DiagnosticSeverity.Error, true);
                return Diagnostic.Create(desc, null);
            }
        }
    }
}
