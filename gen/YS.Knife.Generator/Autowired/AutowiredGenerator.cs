using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YS.Knife
{
    [Generator]
    public class AutowiredGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new AutowiredSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {

        }

        internal class AutowiredSyntaxReceiver : ISyntaxReceiver
        {
            public IList<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();
            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {

                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    var hasInstanceFieldWithAttribute = classDeclarationSyntax.Members.OfType<FieldDeclarationSyntax>()
                        .Any(HasInstanceFieldAndDefinedAttribute);
                    if (hasInstanceFieldWithAttribute)
                    {
                        CandidateClasses.Add(classDeclarationSyntax);
                    }
                }

                bool HasInstanceFieldAndDefinedAttribute(FieldDeclarationSyntax fieldDeclarationSyntax)
                {
                    return fieldDeclarationSyntax.AttributeLists.Any() &&
                           !fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword) &&
                           !fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.ConstKeyword);
                }
            }
        }
    }
}
