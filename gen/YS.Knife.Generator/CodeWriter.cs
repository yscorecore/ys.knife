using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace YS.Knife
{
    class CodeWriter
    {
        public CodeWriter(GeneratorExecutionContext context)
        {
            Context = context;
            this.Compilation = context.Compilation;
        }
        public string CodeFileSuffix { get; set; } = "g.cs";
        public Compilation Compilation { get; private set; }
        private GeneratorExecutionContext Context { get; }

        private Dictionary<string, int> fileNames = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);

        public void WriteCodeFile(CodeFile codeFile)
        {

            fileNames.TryGetValue(codeFile.BasicName, out var i);
            var name = i == 0 ? codeFile.BasicName : $"{codeFile.BasicName}.{i + 1}";
            fileNames[codeFile.BasicName] = i + 1;

            this.Context.AddSource($"{name}.{CodeFileSuffix}", codeFile.Content);

            this.Compilation = this.Compilation.AddSyntaxTrees(
               CSharpSyntaxTree.ParseText(SourceText.From(codeFile.Content, Encoding.UTF8)));
        }
    }
}
