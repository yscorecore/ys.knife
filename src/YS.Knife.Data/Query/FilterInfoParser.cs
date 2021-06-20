using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace YS.Knife.Data
{
    public class FilterInfoParser
    {
        Func<char, bool> IsWhiteSpace = ch => ch == ' ' || ch == '\t';
        Func<char, bool> IsValidNameFirstChar = ch => char.IsLetter(ch) || ch == '_';
        Func<char, bool> IsValidNameChar = ch => char.IsLetterOrDigit(ch) || ch == '_';
        Func<char, bool> IsOperationChar = ch => ch == '=' || ch == '<' || ch == '>' || ch == '!';
        Func<char, bool> IsEscapeChar = ch => ch == '\\';
        internal static readonly Dictionary<string, FilterType> FilterTypeCodes = new Dictionary<string, FilterType>
        {
            ["="] = FilterType.Equals,
            ["=="] = FilterType.Equals,
            ["!="] = FilterType.NotEquals,
            ["<>"] = FilterType.NotEquals,
            [">"] = FilterType.GreaterThan,
            [">="] = FilterType.GreaterThanOrEqual,
            ["<"] = FilterType.LessThan,
            ["<="] = FilterType.LessThanOrEqual,
            ["<="] = FilterType.LessThanOrEqual,
            ["in"] = FilterType.In,
            ["nin"] = FilterType.NotIn,
            ["bt"] = FilterType.Between,
            ["nbt"] = FilterType.NotBetween,

            ["ct"] = FilterType.Contains,
            ["nct"] = FilterType.NotContains,

            ["sw"] = FilterType.StartsWith,
            ["nsw"] = FilterType.NotStartsWith,

            ["ew"] = FilterType.EndsWith,
            ["new"] = FilterType.NotEndsWith,
        };
        internal static readonly Dictionary<string, object> KeyWordValues = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["true"] = true,
            ["false"] = false,
            ["null"] = null,
            ["undefined"] = null,
        };

        public FilterInfo Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return null; }
            var context = new ParseContext(text);
            var filterInfo = DoParse(context);
            return filterInfo;
        }
        private FilterInfo DoParse(ParseContext context)
        {
            SkipWhiteSpace(context);



            return ParseSingleItemOne(context);
        }
        private FilterInfo ParseSingleItemOne(ParseContext context)
        {
            var (field, func) = ParseFieldPath(context);

            var type = ParseType(context);

            var value = ParseValue(context);

            return new FilterInfo()
            {
                OpType = OpType.SingleItem,
                FieldName = field,
                FilterType = type,
                Function = func,
                Value = value
            };

        }
        private void SkipWhiteSpace(ParseContext context)
        {
            while (context.Index < context.TotalLength)
            {
                if (IsWhiteSpace(context.Current()))
                {
                    context.Index++;
                }
                else
                {
                    break;
                }
            }
        }
        private (string Field, CollectionFunctionInfo Function) ParseFieldPath(ParseContext context)
        {
            SkipWhiteSpace(context);
            List<string> names = new List<string>();
            while (context.NotEnd())
            {
                var name = ParseFieldName(context);
                if (context.Current() == '.')
                {
                    names.Add(name);
                    context.Index++;

                }
                else if (context.Current() == '(')
                {


                    CollectionFunctionInfo functionInfo = new CollectionFunctionInfo();
                    functionInfo.FuncName = name;
                    functionInfo.SubFilter = DoParse(context);

                    // TODO ..

                    return (JoinNames(names), functionInfo);
                }
                else
                {
                    names.Add(name);
                    return (JoinNames(names), null);
                }

            }
            throw ParseErrors.InvalidText(context);

        }
        private string JoinNames(IEnumerable<string> names)
        {
            return string.Join('.', names);
        }
        private string ParseFieldName(ParseContext context)
        {
            SkipWhiteSpace(context);
            int startIndex = context.Index;
            if (!IsValidNameFirstChar(context.Current()))
            {
                throw ParseErrors.InvalidFieldNameText(context);
            }
            context.Index++;// first char

            while (context.NotEnd() && IsValidNameChar(context.Current()))
            {
                context.Index++;
            }

            return context.Text.Substring(startIndex, context.Index - startIndex);
        }

        private FilterType ParseType(ParseContext context)
        {
            SkipWhiteSpace(context);
            int startIndex = context.Index;
            if (char.IsLetter(context.Current()))
            {
                while (context.NotEnd() && IsValidNameChar(context.Current()))
                {
                    context.Index++;
                }
                string opCode = context.Text.Substring(startIndex, context.Index - startIndex);
                if (FilterTypeCodes.TryGetValue(opCode.ToLowerInvariant(), out FilterType filterType))
                {
                    return filterType;
                }
                else
                {
                    throw ParseErrors.InvalidFilterType(context, opCode);
                }
            }
            else if (IsOperationChar(context.Current()))
            {
                while (context.NotEnd() && IsOperationChar(context.Current()))
                {
                    context.Index++;
                }
                string opCode = context.Text.Substring(startIndex, context.Index - startIndex);
                if (FilterTypeCodes.TryGetValue(opCode, out FilterType filterType))
                {
                    return filterType;
                }
                else
                {
                    throw ParseErrors.InvalidFilterType(context, opCode);
                }
            }
            else
            {
                throw ParseErrors.InvalidFilterType(context);
            }
        }
        private object ParseValue(ParseContext context)
        {
            SkipWhiteSpace(context);
            var current = context.Current();
            if (current == '\"')
            {
                //string
                return ParseStringValue(context);
            }
            else if (char.IsLetter(current))
            {
                //keyword
                return ParseKeywordValue(context);
            }
            else if (char.IsDigit(current) || current == '.')
            {
                //number
                return ParseNumberValue(context);
            }
            else
            {
                throw ParseErrors.InvalidValue(context);
            }
        }
        private string ParseStringValue(ParseContext context)
        {
            // skip start
            context.Index++;
            bool lastIsEscapeChar = false;
            var startIndex = context.Index;

            while (context.NotEnd())
            {
                if (lastIsEscapeChar)
                {
                    lastIsEscapeChar = false;
                    context.Index++;
                    continue;
                }
                else
                {
                    var current = context.Current();
                    if (current == '\"')
                    {
                        break;
                    }
                    lastIsEscapeChar = IsEscapeChar(current);
                    context.Index++;
                }

            }
            string origin = context.Text.Substring(startIndex, context.Index - startIndex);
            // skip end
            context.Index++;
            try
            {
                return Regex.Unescape(origin);
            }
            catch (Exception ex)
            {
                throw ParseErrors.InvalidStringValue(context, origin, ex);
            }
        }
        private object ParseNumberValue(ParseContext context)
        {
            // TODO ...
            return null;
        }
        private object ParseKeywordValue(ParseContext context)
        {
            int startIndex = context.Index;
            while (context.NotEnd() && IsValidNameChar(context.Current()))
            {
                context.Index++;
            }
            string keyWord = context.Text.Substring(startIndex, context.Index - startIndex);
            if (KeyWordValues.TryGetValue(keyWord, out object value))
            {
                return value;
            }
            else
            {
                throw ParseErrors.InvalidKeywordValue(context, keyWord);
            }
        }

        private void ThrowIfEnd(ParseContext context)
        {
            throw ParseErrors.InvalidText(context);
        }

        class ParseContext
        {
            public ParseContext(string text)
            {
                this.Text = text;
                this.TotalLength = text.Length;
            }
            public char Current()
            {
                if (Index >= TotalLength)
                {
                    throw ParseErrors.InvalidText(this);
                }
                return Text[Index];
            }
            public bool NotEnd()
            {
                return Index < TotalLength;
            }
            public string Text;
            public int TotalLength;
            public int Index;
        }
        class ParseErrors
        {
            public static Exception InvalidText(ParseContext context)
            {
                throw new Exception($"Invalid text near index {context.Index}.");
            }
            public static Exception InvalidFieldNameText(ParseContext context)
            {
                throw new Exception($"Invalid field name near index {context.Index}.");
            }
            public static Exception InvalidFilterType(ParseContext context)
            {
                throw new Exception($"Invalid filter type near index {context.Index}.");
            }
            public static Exception InvalidFilterType(ParseContext context, string code)
            {
                throw new Exception($"Invalid filter type '{code}' near index {context.Index}.");
            }
            public static Exception InvalidValue(ParseContext context)
            {
                throw new Exception($"Invalid value near index {context.Index}.");
            }
            public static Exception InvalidKeywordValue(ParseContext context, string keyword)
            {
                throw new Exception($"Invalid keyword '{keyword}' near index {context.Index}.");
            }
            public static Exception InvalidStringValue(ParseContext context, string str, Exception inner)
            {
                throw new Exception($"Invalid string '{str}' near index {context.Index}.", inner);
            }
        }
    }

}
