﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

// ReSharper disable once CheckNamespace
namespace YS.Knife
{
    [Generator]
    public class AutoNotifyGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new AutoNotifySyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is AutoNotifySyntaxReceiver receiver))
                return;

            var codeWriter = new CodeWriter(context);

            foreach (var clazzSymbol in codeWriter.GetAllClassSymbolsIgnoreRepeated(receiver.CandidateClasses))
            {
                var fieldList = clazzSymbol.GetAllInstanceFieldsByAttribute(typeof(AutoNotifyAttribute)).ToList();
                if (fieldList.Any())
                {
                    var codeFile = ProcessClass(clazzSymbol, fieldList, codeWriter.Compilation);
                    codeWriter.WriteCodeFile(codeFile);
                }
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("MicrosoftCodeAnalysisCorrectness",
            "RS1024:Compare symbols correctly", Justification = "<Pending>")]
        private CodeFile ProcessClass(INamedTypeSymbol classSymbol, List<IFieldSymbol> fields, Compilation compilation)
        {
            INamedTypeSymbol notifySymbol =
                compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged");

            var classSymbols = classSymbol.GetParentClassChains();


            CsharpCodeBuilder codeBuilder = new CsharpCodeBuilder();

            var allNamespaces = new HashSet<string>();

            allNamespaces.Add("System.ComponentModel");

            foreach (var field in fields)
            {
                if (!field.Type.ContainingNamespace.IsGlobalNamespace)
                {
                    allNamespaces.Add(field.Type.ContainingNamespace.ToDisplayString());
                }
            }

            allNamespaces.Remove(classSymbol.ContainingNamespace.ToDisplayString());

            foreach (var usingNamespace in allNamespaces.OrderBy(p => p))
            {
                codeBuilder.AppendCodeLines($"using {usingNamespace};");
            }

            //codeBuilder.AppendCodeLines($@"using System.ComponentModel;");
            if (!classSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                codeBuilder.AppendCodeLines($"namespace {classSymbol.ContainingNamespace.ToDisplayString()}");
                codeBuilder.BeginSegment();
            }

            foreach (var parentClass in classSymbols)
            {
                if (parentClass != classSymbol)
                {
                    codeBuilder.AppendCodeLines($@"partial class {parentClass.Name}");
                    codeBuilder.BeginSegment();
                }
            }


            if (!classSymbol.AllInterfaces.Contains(notifySymbol))
            {
                codeBuilder.AppendCodeLines(
                    $@"partial class {classSymbol.Name} : {notifySymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}");
                codeBuilder.BeginSegment();
                codeBuilder.AppendCodeLines("public event PropertyChangedEventHandler PropertyChanged;");
            }
            else
            {
                codeBuilder.AppendCodeLines($@"partial class {classSymbol.Name}");
                codeBuilder.BeginSegment();
            }

            // create properties for each field 
            foreach (IFieldSymbol fieldSymbol in fields)
            {
                ProcessField(codeBuilder, fieldSymbol);
            }

            codeBuilder.EndAllSegments();
            return new CodeFile
            {
                BasicName = string.Join(".", classSymbols.Select(p => p.Name)), Content = codeBuilder.ToString(),
            };
        }

        private static void ProcessField(CsharpCodeBuilder source, IFieldSymbol fieldSymbol)
        {
            // get the name and type of the field
            string fieldName = fieldSymbol.Name;
            ITypeSymbol fieldType = fieldSymbol.Type;

            AttributeData attributeData = fieldSymbol.GetAttributes()
                .Single(p => p.AttributeClass.SafeEquals(typeof(AutoNotifyAttribute)));
            // get the AutoNotify attribute from the field, and any associated data

            TypedConstant overridenNameOpt =
                attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "PropertyName").Value;

            string propertyName = chooseName(fieldName, overridenNameOpt);
            if (propertyName.Length == 0 || propertyName == fieldName)
            {
                //TODO: issue a diagnostic that we can't process this field
                return;
            }

            source.AppendCodeLines($@"
public {fieldType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} {propertyName} 
{{
    get 
    {{
        return this.{fieldName};
    }}
    set
    {{
        if(this.{fieldName} != value)
        {{
            this.{fieldName} = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof({propertyName})));
        }}
    }}
}}

");

            string chooseName(string field, TypedConstant overridenNameOption)
            {
                if (!overridenNameOption.IsNull)
                {
                    return overridenNameOption.Value.ToString();
                }

                field = field.TrimStart('_');
                if (field.Length == 0)
                    return string.Empty;

                if (field.Length == 1)
                    return field.ToUpperInvariant();

                return field.Substring(0, 1).ToUpperInvariant() + field.Substring(1);
            }
        }

        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        class AutoNotifySyntaxReceiver : ISyntaxReceiver
        {
            public IList<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax
                    && fieldDeclarationSyntax.AttributeLists.Any() &&
                    !fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                    !fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    if (fieldDeclarationSyntax.Parent is ClassDeclarationSyntax clazzSyntax && !CandidateClasses.Contains(clazzSyntax))
                    {
                        CandidateClasses.Add((clazzSyntax));
                    }

                }
            }
        }
    }
}
