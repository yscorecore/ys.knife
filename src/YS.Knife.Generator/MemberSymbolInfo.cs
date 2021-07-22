using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace YS.Knife.Generator
{
    internal class MemberSymbolInfo
    {
        public string Type { get; set; }
        public string ParameterName { get; set; }
        public string Name { get; set; }
        public IEnumerable<AttributeData> Attributes { get; set; }
    }
}
