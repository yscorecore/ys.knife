using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;

namespace YS.Knife
{
    [Generator]
    public class AutoNotifyGenerator : ISourceGenerator
    {
        private const string attributeText = @"
using System;
namespace YS.Knife
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class AutoNotifyAttribute : Attribute
    {
        public AutoNotifyAttribute()
        {
        }
        public string PropertyName { get; set; }
    }
}
";

        public void Initialize(GeneratorInitializationContext context)
        {
            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "<Pending>")]
        public void Execute(GeneratorExecutionContext context)
        {
            // add the attribute text
            context.AddSource("AutoNotifyAttribute", attributeText);

            // retreive the populated receiver 
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;

            // we're going to create a new compilation that contains the attribute.
            // TODO: we should allow source generators to provide source during initialize, so that this step isn't required.
            CSharpParseOptions options = (context.Compilation as CSharpCompilation).SyntaxTrees[0].Options as CSharpParseOptions;
            Compilation compilation = context.Compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(attributeText, Encoding.UTF8), options));

            // get the newly bound attribute, and INotifyPropertyChanged
            INamedTypeSymbol attributeSymbol = compilation.GetTypeByMetadataName("YS.Knife.AutoNotifyAttribute");
            INamedTypeSymbol notifySymbol = compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged");

            // loop over the candidate fields, and keep the ones that are actually annotated
            List<IFieldSymbol> fieldSymbols = new List<IFieldSymbol>();
            foreach (FieldDeclarationSyntax field in receiver.CandidateFields)
            {
                SemanticModel model = compilation.GetSemanticModel(field.SyntaxTree);
                foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
                {
                    // Get the symbol being decleared by the field, and keep it if its annotated
                    IFieldSymbol fieldSymbol = model.GetDeclaredSymbol(variable) as IFieldSymbol;
                    if (fieldSymbol.GetAttributes().Any(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default)))
                    {
                        fieldSymbols.Add(fieldSymbol);
                    }
                }
            }

          
            // group the fields by class, and generate the source
            foreach (IGrouping<INamedTypeSymbol, IFieldSymbol> group in fieldSymbols.GroupBy(f => f.ContainingType))
            {
                var nameChains = GetClassNameChains(group.Key);
                
                string classSource = ProcessClass(nameChains, group.ToList(), attributeSymbol, notifySymbol, context);
                
                context.AddSource($"{string.Join(".",nameChains.Select(p=>p.Name))}.AutoNotify.cs", classSource);
            }
        }
        private IList<INamedTypeSymbol> GetClassNameChains(INamedTypeSymbol classSymbol)
        {
            var namespaceSymbol = classSymbol.ContainingNamespace;
            List<INamedTypeSymbol> paths = new List<INamedTypeSymbol>();
            while (classSymbol!=null)
            {

                paths.Insert(0,classSymbol);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1024:Compare symbols correctly", Justification = "<Pending>")]
        private string ProcessClass(IList<INamedTypeSymbol> classSymbols, List<IFieldSymbol> fields, ISymbol attributeSymbol, ISymbol notifySymbol, GeneratorExecutionContext context)
        {
            var classSymbol = classSymbols.Last();
            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            CsharpCodeBuilder codeBuilder = new CsharpCodeBuilder();
            codeBuilder.AppendThenIncrease($@"using System.ComponentModel;
namespace {namespaceName}
{{");
            foreach (var parentClass in classSymbols)
            {
                if (parentClass != classSymbol)
                {
                    codeBuilder.AppendThenIncrease($@"partial class {parentClass.Name}
{{");
                }
            }


            codeBuilder.AppendThenIncrease($@"partial class {classSymbol.Name} : {notifySymbol.ToDisplayString()}
{{");

            if (!classSymbol.Interfaces.Contains(notifySymbol))
            {
                codeBuilder.AppendCodeLines("public event PropertyChangedEventHandler PropertyChanged;");
            }

            // create properties for each field 
            foreach (IFieldSymbol fieldSymbol in fields)
            {
                ProcessField(codeBuilder, fieldSymbol, attributeSymbol);
            }
            codeBuilder.DecreaseDepth();
            for (int i = 0; i <= classSymbols.Count; i++)
            {
                codeBuilder.AppendThenDecrease("}");
            }

            return codeBuilder.ToString();
          
        }

        private static void ProcessField(CsharpCodeBuilder source, IFieldSymbol fieldSymbol, ISymbol attributeSymbol)
        {
            // get the name and type of the field
            string fieldName = fieldSymbol.Name;
            ITypeSymbol fieldType = fieldSymbol.Type;

            // get the AutoNotify attribute from the field, and any associated data
            AttributeData attributeData = fieldSymbol.GetAttributes().Single(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default));
            TypedConstant overridenNameOpt = attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "PropertyName").Value;

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
        class SyntaxReceiver : ISyntaxReceiver
        {
            public List<FieldDeclarationSyntax> CandidateFields { get; } = new List<FieldDeclarationSyntax>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                // any field with at least one attribute is a candidate for property generation
                if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax
                    && fieldDeclarationSyntax.AttributeLists.Count > 0)
                {
                    CandidateFields.Add(fieldDeclarationSyntax);
                }
            }
        }
    }
}
