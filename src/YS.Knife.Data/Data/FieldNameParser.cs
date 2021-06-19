using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace YS.Knife.Data
{
    internal class FieldNameParser
    {
        public static string[] Parse(string fieldName)
        {
            var names = fieldName.Split('.').Select(p => p.Trim()).ToArray();
            Array.ForEach(names, (name) =>
            {
                if (!IsValidMemberName(name))
                {
                    throw Errors.InvalidFieldName(fieldName);
                }
            });
            return names;
        }
        
        static readonly Regex MemberNameRegex = new Regex(@"^\w+$");
        public static bool IsValidMemberName(string name)
        {
            return MemberNameRegex.IsMatch(name);
        }
    }
}
