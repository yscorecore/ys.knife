using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace YS.Knife
{
    [Generator]
    class ConvertToGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ConvertToSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxReceiver is ConvertToSyntaxReceiver receiver))
                return;
            var codeWriter = new CodeWriter(context);

            codeWriter.ForeachClassSyntax(receiver.CandidateClasses, ProcessClass);
        }

        private CodeFile ProcessClass(INamedTypeSymbol classSymbol, CodeWriter codeWriter)
        {
            if (!classSymbol.HasAttribute(typeof(ConvertToAttribute)))
            {
                return null;
            }

            CsharpCodeBuilder codeBuilder = new CsharpCodeBuilder();
            AppendUsingLines(classSymbol, codeBuilder);
            AppendNamespace(classSymbol, codeBuilder);
            AppendClassDefinition(classSymbol, codeBuilder);
            foreach (var convertToAttr in classSymbol.GetAttributes().Where(p => p.AttributeClass.SafeEquals(typeof(ConvertToAttribute))))
            {
                AppendConvertToFunctions(convertToAttr, codeBuilder);
            }

            codeBuilder.EndAllSegments();
            return new CodeFile
            {
                BasicName = classSymbol.GetCodeFileBasicName(),
                Content = codeBuilder.ToString(),
            };


        }
        void AppendUsingLines(INamedTypeSymbol _, CsharpCodeBuilder codeBuilder)
        {
            codeBuilder.AppendCodeLines("using System.Collections.Generic;");
            codeBuilder.AppendCodeLines("using System.Linq;");
        }
        void AppendNamespace(INamedTypeSymbol classSymbol, CsharpCodeBuilder codeBuilder)
        {
            if (!classSymbol.ContainingNamespace.IsGlobalNamespace)
            {
                codeBuilder.AppendCodeLines($"namespace {classSymbol.ContainingNamespace.ToDisplayString()}");
                codeBuilder.BeginSegment();
            }
        }
        void AppendClassDefinition(INamedTypeSymbol classSymbol, CsharpCodeBuilder codeBuilder)
        {
            var classSymbols = classSymbol.GetContainerClassChains();
            for (int i = 0; i < classSymbols.Count; i++)
            {
                var parentClass = classSymbols[i];
                if (i == classSymbols.Count - 1)
                {
                    codeBuilder.AppendCodeLines($@"static partial class {parentClass.GetClassSymbolDisplayText()}");
                }
                else
                {
                    codeBuilder.AppendCodeLines($@"partial class {parentClass.GetClassSymbolDisplayText()}");
                }
                codeBuilder.BeginSegment();
            }
        }

        void AppendConvertToFunctions(AttributeData attributeData, CsharpCodeBuilder codeBuilder)
        {
            var v1= attributeData.NamedArguments.FirstOrDefault();
            var v2 = attributeData.ConstructorArguments.FirstOrDefault();
            /**
                     public static IEnumerable<UserInfo> ToUserInfo(this IEnumerable<TUser> user)
        {
            return user?.Select(p => new UserInfo
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age
            });
        }
        public static IQueryable<UserInfo> ToUserInfo(this IQueryable<TUser> user)
        {
            return user?.Select(p => new UserInfo
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age
            });
        }
        public static UserInfo ToUserInfo(this TUser user)
        {
            if (user == null) return null;
            return new UserInfo
            {
                Id = user.Id,
                Name = user.Name,
                Age = user.Age
            };
        }
        public static void ToUserInfo(this TUser user, UserInfo userInfo)
        {
            if (userInfo != null && user != null)
            {
                userInfo.Id = user.Id;
                userInfo.Age = user.Age;
                userInfo.Name = user.Name;
            }
        }
             */
        }

        private class ConvertToSyntaxReceiver : ISyntaxReceiver
        {
            public IList<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax &&
                    classDeclarationSyntax.AttributeLists.Any())
                {
                    CandidateClasses.Add(classDeclarationSyntax);
                }
            }
        }
    }
}
