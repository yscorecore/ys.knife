using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
namespace YS.Knife.Query
{
    class QueryUtils
    {
        public static string CheckFieldName(string fieldName)
        {
            if (!Regex.IsMatch(fieldName ?? string.Empty, @"^\w+(\.\w+)*$"))
            {
                throw new FormatException($"Invalid field name format, \"{fieldName}\".");
            }
            return fieldName;
        }
    }
}
