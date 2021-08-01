﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace YS.Knife
{
    [Generator]
    public class AutoNotifyGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new AutoNotifySyntaxReceiver());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("MicrosoftCodeAnalysisCorrectness",
            "RS1024:Compare symbols correctly", Justification = "<Pending>")]
        public void Execute(GeneratorExecutionContext context)
        {



            // retreive the populated receiver 
            if (!(context.SyntaxReceiver is AutoNotifySyntaxReceiver receiver))
                return;

            Compilation compilation = context.Compilation;

            // get the newly bound attribute, and INotifyPropertyChanged

            INamedTypeSymbol attributeSymbol = compilation.GetTypeByMetadataName("YS.Knife.AutoNotifyAttribute");

            if (attributeSymbol == null)
            {
                context.ReportDiagnostic(KnifeDiagnostic.NotRefKnifeGeneratorAssembly());
            }

            INamedTypeSymbol notifySymbol =
                compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged");

            var fileNames = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var clazz in receiver.CandidateClasses)
            {
                SemanticModel model = compilation.GetSemanticModel(clazz.SyntaxTree);
                var clazzSymbol = model.GetDeclaredSymbol(clazz);
                var fieldList = clazzSymbol.GetMembers().OfType<IFieldSymbol>()
                        .Where(p => p.CanBeReferencedByName && !p.IsStatic && p.HasAttribute(attributeSymbol))
                        .ToList();
                if (fieldList.Count == 0)
                {
                    continue;
                }
                var nameChains = GetClassNameChains(clazzSymbol);

                string classSource = ProcessClass(nameChains, fieldList, attributeSymbol, notifySymbol, context);

                string fileNamePrefix = string.Join(".", nameChains.Select(p => p.Name));

                fileNames.TryGetValue(fileNamePrefix, out var i);
                var name = i == 0 ? fileNamePrefix : $"{fileNamePrefix}.{i + 1}";
                fileNames[fileNamePrefix] = i + 1;

                context.AddSource($"{name}.AutoNotify.g.cs", classSource);


                compilation= context.Compilation.AddSyntaxTrees(
                    CSharpSyntaxTree.ParseText(SourceText.From(classSource, Encoding.UTF8)));

            }

        }

        private IList<INamedTypeSymbol> GetClassNameChains(INamedTypeSymbol classSymbol)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("MicrosoftCodeAnalysisCorrectness",
            "RS1024:Compare symbols correctly", Justification = "<Pending>")]
        private string ProcessClass(IList<INamedTypeSymbol> classSymbols, List<IFieldSymbol> fields,
            ISymbol attributeSymbol, ISymbol notifySymbol, GeneratorExecutionContext context)
        {
            var classSymbol = classSymbols.Last();

            CsharpCodeBuilder codeBuilder = new CsharpCodeBuilder();
            codeBuilder.AppendCodeLines($@"using System.ComponentModel;");
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
                codeBuilder.AppendCodeLines($@"partial class {classSymbol.Name} : {notifySymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}");
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
                ProcessField(codeBuilder, fieldSymbol, attributeSymbol);
            }

            codeBuilder.EndAllSegments();
            return codeBuilder.ToString();


        }

        private static void ProcessField(CsharpCodeBuilder source, IFieldSymbol fieldSymbol, ISymbol attributeSymbol)
        {
            // get the name and type of the field
            string fieldName = fieldSymbol.Name;
            ITypeSymbol fieldType = fieldSymbol.Type;

            // get the AutoNotify attribute from the field, and any associated data
            AttributeData attributeData = fieldSymbol.GetAttributes().Single(ad =>
                ad.AttributeClass.SafeEquals(attributeSymbol));
            TypedConstant overridenNameOpt =
                attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "PropertyName").Value;

            string propertyName = chooseName(fieldName, overridenNameOpt);
            if (propertyName.Length == 0 || propertyName == fieldName)
            {
                //TODO: issue a diagnostic that we can't process this field
                return;
            }

            source.AppendCodeLines($@"
public {fieldType} {propertyName} 
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
            //public List<FieldDeclarationSyntax> CandidateFields { get; } = new List<FieldDeclarationSyntax>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                // any field with at least one attribute is a candidate for property generation
                //if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax
                //    && fieldDeclarationSyntax.AttributeLists.Count > 0)
                //{
                //    CandidateFields.Add(fieldDeclarationSyntax);
                //}
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    CandidateClasses.Add(classDeclarationSyntax);
                }
            }
        }
    }
}
