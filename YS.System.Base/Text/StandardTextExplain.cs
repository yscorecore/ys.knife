using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Varriables;
using System.Text.RegularExpressions;
namespace System.Text
{
    public sealed class StandardTextExplain:IVarriableExplain
    {
        private const string Pattern = @"\\(?<ch>[abfnrtv\\'""])|\\x(?<hex>[0-9A-Fa-f]{2})|\\u(?<unicode>[0-9A-Fa-f]{4})|\\(?<zero>0)";
        private const string ChName = "ch";
        private const string HexName = "hex";
        private const string UnicodeName = "unicode";
        private const string ZeroName = "zero";
       
        private static StandardTextExplain myobject;

        private StandardTextExplain()
        {
        }

        public static StandardTextExplain Instance
        {
            get
            {
                if(object.ReferenceEquals(myobject,null))
                {
                    myobject = new StandardTextExplain();
                }
                return myobject;
            }
        }
        public object ExpandEnvironmentVariables(string variableStr)
        {
            if(string.IsNullOrEmpty(variableStr))
            {
                return string.Empty;
            }
            else
            {
                return Regex.Replace(variableStr,Pattern,MatchEvaluator1);
            }
        }

        private string MatchEvaluator1(Match match)
        {
            if(match.Groups[ChName].Success)
            {
                return ChToChar(match.Groups[ChName].Value);
            }
            else if(match.Groups[HexName].Success)
            {
                return HexToChar(match.Groups[HexName].Value);
            }
            else if(match.Groups[UnicodeName].Success)
            {
                return UnicodeToChar(match.Groups[UnicodeName].Value);
            }
            else if(match.Groups[ZeroName].Success)
            {
                return "\0";
            }
            else
            {
                return match.Value;
            }
        }

        private string ChToChar(string ch)
        {
            switch(ch)
            {
                case "a":
                    return "\a";
                case "b":
                    return "\b";
                 case "f":
                    return "\f";
                 case "n":
                    return "\n";
                 case "r":
                    return "\r";
                case "t":
                    return "\t";
                 case "v":
                    return "\v";
                 case "\\":
                    return "\\";
                case "\"":
                    return "\"";
                case "'":
                    return "'";
                default:
                    return ch;
            }
        }

        private string HexToChar(string hex)
        {
            return ((char)(Convert.ToInt32(hex,16))).ToString();
        }

        private string UnicodeToChar(string unicode)
        {
            return char.ConvertFromUtf32(Convert.ToInt32(unicode,16));
        }

        public static string DecodeEscape(string escapeText)
        {
            return Instance.ExpandEnvironmentVariables(escapeText) as string;
        }
    }
}
