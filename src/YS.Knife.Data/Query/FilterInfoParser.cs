using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YS.Knife.Data
{
    internal class FilterInfoParser
    {
        static readonly Func<char, bool> IsWhiteSpace = ch => ch == ' ' || ch == '\t';
        static readonly Func<char, bool> IsValidNameFirstChar = ch => char.IsLetter(ch) || ch == '_';
        static readonly Func<char, bool> IsValidNameChar = ch => char.IsLetterOrDigit(ch) || ch == '_';
        static readonly Func<char, bool> IsOperationChar = ch => ch == '=' || ch == '<' || ch == '>' || ch == '!';
        static readonly Func<char, bool> IsEscapeChar = ch => ch == '\\';

        internal static readonly Dictionary<string, FilterType> FilterTypeCodes =
            FilterInfo.FilterTypeNameMapper.Select(p => Tuple.Create(p.Value, p.Key))
            .Concat(new[] {
                Tuple.Create("=",FilterType.Equals),
                Tuple.Create("<>",FilterType.NotEquals)
            
            }).ToDictionary(p => p.Item1, p => p.Item2);
        internal static readonly Dictionary<string, object> KeyWordValues = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["true"] = true,
            ["false"] = false,
            ["null"] = null
        };
        internal static readonly Dictionary<string, OpType> OpTypeCodes = new Dictionary<string, OpType>(StringComparer.InvariantCultureIgnoreCase)
        {
            [FilterInfo.Operator_And] = OpType.AndItems,
            [FilterInfo.Operator_Or] = OpType.OrItems
        };

        private readonly char _numberDecimal; // 小数点
        private readonly char _numberNegativeSign;// 负号
        private readonly char _numberPositiveSign; // 正号
        private readonly char _numberGroupSeparator;// 分组符号
        private readonly CultureInfo _currentCulture;
        public FilterInfoParser(CultureInfo cultureInfo)
        {
            this._currentCulture = cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo));
            this._numberDecimal = cultureInfo.NumberFormat.NumberDecimalSeparator[0];
            this._numberNegativeSign = cultureInfo.NumberFormat.NegativeSign[0];
            this._numberPositiveSign = cultureInfo.NumberFormat.PositiveSign[0];
            // this._numberGroupSeparator = cultureInfo.NumberFormat.NumberGroupSeparator[0];
            // default number group separator will conflict with array separator
            // eg. [1,234], so use '_' instead of default number group separator
            this._numberGroupSeparator = '_';
        }
        public FilterInfo Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return null; }
            var context = new ParseContext(text);
            var filterInfo = ParseFilterExpression(context);
            SkipWhiteSpace(context);
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return filterInfo;
        }
        private FilterInfo ParseFilterExpression(ParseContext context)
        {
            SkipWhiteSpace(context);
            if (context.Current() == '(')
            {
                return ParseCombinFilter(context);
            }
            else
            {
                return ParseSingleItemOne(context);
            }
        }
        private FilterInfo ParseCombinFilter(ParseContext context)
        {
            List<FilterInfo> orItems = new List<FilterInfo>();
            OpType lastOpType = OpType.OrItems;
            while (context.NotEnd())
            {
                // skip start bracket
                SkipWhiteSpace(context);
                if (context.Current() != '(')
                {
                    throw ParseErrors.MissOpenBracket(context);
                }
                context.Index++;
                SkipWhiteSpace(context);
                var inner = ParseFilterExpression(context);
                SkipWhiteSpace(context);
                if (context.Current() != ')')
                {
                    throw ParseErrors.MissCloseBracket(context);
                }
                context.Index++;

                if (lastOpType == OpType.OrItems || orItems.Count == 0)
                {
                    orItems.Add(inner);
                }
                else
                {
                    orItems[^1] = orItems[^1].AndAlso(inner);
                }

                OpType? opType = TryParseOpType(context);

                if (opType == null)
                {
                    break;
                }
                else
                {
                    lastOpType = opType.Value;
                }
            }
            return orItems.Count > 1 ? new FilterInfo(orItems, OpType.OrItems) : orItems.FirstOrDefault();
        }
        private OpType? TryParseOpType(ParseContext context)
        {
            var originIndex = context.Index;

            SkipWhiteSpace(context);
            var wordStartIndex = context.Index;
            while (context.NotEnd() && IsValidNameChar(context.Current()))
            {
                context.Index++;
            }
            var word = context.Text.Substring(wordStartIndex, context.Index - wordStartIndex);
            if (OpTypeCodes.TryGetValue(word, out var opType))
            {
                return opType;
            }
            else
            {
                // reset index
                context.Index = originIndex;
                return null;
            }
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

        private void SkipCloseBracket(ParseContext context)
        {
            SkipWhiteSpace(context);
            if (context.Current() != ')')
            {
                throw ParseErrors.MissCloseBracket(context);
            }
            else
            {
                context.Index++;
            }
        }
        private (string Field, FunctionInfo Function) ParseFieldPath(ParseContext context)
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
                    FunctionInfo functionInfo = ParseFunctionBody(context);
                    functionInfo.Name = name;
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

        private string ParseNameChain(ParseContext context)
        {
            List<string> names = new List<string>();
            while (context.NotEnd())
            {
                names.Add(ParseFieldName(context));
                if (context.Current() == '.')
                {
                    context.Index++;

                }
                else
                {
                    break;
                }
            }
            return JoinNames(names);
        }
        private FunctionInfo ParseFunctionBody(ParseContext context)
        {
            FunctionInfo functionInfo = new FunctionInfo();
            // skip start (
            context.Index++;
            SkipWhiteSpace(context);
            var current = context.Current();
            if (current == ')')
            {
                // eg. count();
                context.Index++;
            }
            else if (IsValidNameFirstChar(current))
            {
                var originStartIndex = context.Index;

                var nameChain = ParseNameChain(context);
                SkipWhiteSpace(context);
                if (context.Current() == ')')
                {
                    // eg. avg(user.age)
                    functionInfo.FieldName = nameChain;
                    context.Index++;
                }
                else if (context.Current() == ',')
                {
                    // eg. avg(user.age,user.sex="male")
                    functionInfo.FieldName = nameChain;
                    context.Index++;
                    functionInfo.SubFilter = ParseFilterExpression(context);
                    SkipCloseBracket(context);
                }
                else
                {
                    // eg. count(user.sex="male")
                    // reset index
                    context.Index = originStartIndex;
                    functionInfo.SubFilter = ParseFilterExpression(context);
                    SkipCloseBracket(context);
                }
            }
            else if (current == '(')
            {
                // count((user.age>3) and (user.age<10))
                // in this case ,no field name
                functionInfo.SubFilter = ParseFilterExpression(context);
                SkipCloseBracket(context);
            }
            else
            {
                throw ParseErrors.InvalidText(context);
            }
            return functionInfo;
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
        private object ParseValue(ParseContext context, bool parseArray = true)
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
            else if (char.IsDigit(current) || current == _numberDecimal || current == _numberNegativeSign || current == _numberPositiveSign)
            {
                //number
                return ParseNumberValue(context);
            }
            else if (parseArray && current == '[')
            {
                return ParseArrayValue(context);
                //array
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
            int startIndex = context.Index;
            bool hasDecimsl = context.Current() == _numberDecimal;
            // skip first char
            context.Index++;
            while (context.NotEnd())
            {
                var current = context.Current();
                if (char.IsDigit(current))
                {
                    context.Index++;
                }
                else if (current == _numberGroupSeparator)
                {
                    if (hasDecimsl)
                    {
                        throw ParseErrors.InvalidValue(context);
                    }
                    context.Index++;
                }
                else if (current == _numberDecimal)
                {
                    if (hasDecimsl)
                    {
                        throw ParseErrors.InvalidValue(context);
                    }
                    else
                    {
                        hasDecimsl = true;
                    }
                    context.Index++;
                }
                else
                {
                    break;
                }
            }
            var numString = context.Text.Substring(startIndex, context.Index - startIndex);
            try
            {
                return double.Parse(numString.Replace(_numberGroupSeparator.ToString(), string.Empty), NumberStyles.Any, _currentCulture);
            }
            catch (Exception ex)
            {
                throw ParseErrors.InvalidNumberValue(context, numString, ex);
            }
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
        private object ParseArrayValue(ParseContext context)
        {
            // skip start [
            List<object> datas = new List<object>();
            context.Index++;
            while (context.NotEnd())
            {
                SkipWhiteSpace(context);
                var current = context.Current();
                if (current == ']')
                {
                    context.Index++;
                    break;
                }
                datas.Add(ParseValue(context, false));
                SkipWhiteSpace(context);
                if (context.Current() == ',')
                {
                    context.Index++;
                }

            }
            return datas.ToArray();
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
                throw new FilterInfoParseException($"Invalid text near index {context.Index}.");
            }
            public static Exception InvalidFieldNameText(ParseContext context)
            {
                throw new FilterInfoParseException($"Invalid field name near index {context.Index}.");
            }
            public static Exception InvalidFilterType(ParseContext context)
            {
                throw new FilterInfoParseException($"Invalid filter type near index {context.Index}.");
            }
            public static Exception InvalidFilterType(ParseContext context, string code)
            {
                throw new FilterInfoParseException($"Invalid filter type '{code}' near index {context.Index}.");
            }
            public static Exception InvalidValue(ParseContext context)
            {
                throw new FilterInfoParseException($"Invalid value near index {context.Index}.");
            }
            public static Exception InvalidKeywordValue(ParseContext context, string keyword)
            {
                throw new FilterInfoParseException($"Invalid keyword '{keyword}' near index {context.Index}.");
            }
            public static Exception InvalidStringValue(ParseContext context, string str, Exception inner)
            {
                throw new FilterInfoParseException($"Invalid string '{str}' near index {context.Index}.", inner);
            }
            public static Exception InvalidNumberValue(ParseContext context, string str, Exception inner)
            {
                throw new FilterInfoParseException($"Invalid number '{str}' near index {context.Index}.", inner);
            }

            public static Exception MissOpenBracket(ParseContext context)
            {
                throw new FilterInfoParseException($"Invalid expression, missing open bracket near index {context.Index}.");
            }
            public static Exception MissCloseBracket(ParseContext context)
            {
                throw new FilterInfoParseException($"Invalid expression, missing close bracket near index {context.Index}.");
            }
        }
    }

}
