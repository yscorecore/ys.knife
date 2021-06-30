using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace YS.Knife.Data
{
    internal class FilterInfoParser2
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
        public FilterInfoParser2(CultureInfo cultureInfo)
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
        public FilterInfo2 Parse(string text)
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
        private FilterInfo2 ParseFilterExpression(ParseContext context)
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
        private FilterInfo2 ParseCombinFilter(ParseContext context)
        {
            List<FilterInfo2> orItems = new List<FilterInfo2>();
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
            return orItems.Count > 1 ? new FilterInfo2{ OpType =  OpType.OrItems, Items = orItems} : orItems.FirstOrDefault();
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

        private FilterInfo2 ParseSingleItemOne(ParseContext context)
        {
            var (field, func) = ParseFieldPath(context);

            var type = ParseType(context);

            var value = ParseValue(context);

            return new FilterInfo2()
            {
                OpType = OpType.SingleItem,
                //FieldName = field,
                FilterType = type,
                //Function = func,
                //Value = value
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

        private ValueInfo ParseValueInfo(ParseContext context)
        {
            return null;
        }

        private (string Field, FunctionInfo Function) ParseFieldPath(ParseContext context)
        {
            SkipWhiteSpace(context);
            List<string> names = new List<string>();
            while (context.NotEnd())
            {
                var name = ParseName(context);
                if (context.Current() == '.')
                {
                    // a.b
                    names.Add(name);
                    context.Index++;

                }
                else if (context.Current() == '?' || context.Current() == '!')
                {
                    // a?.b or a!.b
                    var fieldTail = context.Current();
                    context.Index++;
                    if (context.Current() == '.')
                    {
                        names.Add(name + fieldTail);
                        context.Index++;
                    }
                    else
                    {
                        names.Add(name);
                        context.Index--;
                        break;
                    }
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
                    break;
                }

            }
            return (JoinNames(names), null);

        }

        private string ParseNameChain(ParseContext context)
        {
            // name chain not contains '?', only contains '.'
            // eg.  "a.b" is valid ,"a?.b" is not valid 
            List<string> names = new List<string>();
            while (context.NotEnd())
            {
                names.Add(ParseName(context));
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
            // skip start (
            context.Index++;
            List<object> args = ParseFunctionArguments(context);
            List<string> fields = ParseFunctionFields(context);
            FilterInfo filterInfo = ParseFunctionFilter(context);

            SkipCloseBracket(context);
            return new FunctionInfo
            {
                Args = args,
                FieldNames = fields,
                SubFilter = filterInfo,
            };

            List<object> ParseFunctionArguments(ParseContext context)
            {
                List<object> datas = new List<object>();
                while (context.NotEnd())
                {
                    SkipWhiteSpace(context);
                    if (IsNumberStartChar(context.Current()))
                    {
                        //number
                        datas.Add(ParseNumberValue(context));
                    }
                    else if (context.Current() == '\"')
                    {
                        //string
                        datas.Add(ParseStringValue(context));
                    }
                    SkipWhiteSpace(context);

                    if (context.Current() == ',')
                    {
                        context.Index++;
                    }
                    else
                    {
                        break;
                    }

                }
                return datas.Any() ? datas : null;
            }
            List<string> ParseFunctionFields(ParseContext context)
            {
                List<string> fields = new List<string>();

                while (context.NotEnd())
                {
                    SkipWhiteSpace(context);
                    if (context.Current() == ')')
                    {
                        break;
                    }

                    else if (IsValidNameFirstChar(context.Current()))
                    {
                        var originStartIndex = context.Index;
                        var nameChain = ParseNameChain(context);
                        SkipWhiteSpace(context);
                        if (context.Current() == ',')
                        {
                            fields.Add(nameChain);
                            context.Index++;
                        }
                        else if (context.Current() == ')')
                        {
                            fields.Add(nameChain);
                            break;
                        }
                        else
                        {
                            context.Index = originStartIndex;
                            break;
                        }

                    }
                    else
                    {
                        break;
                    }

                }


                return fields.Any() ? fields : null;
            }
            FilterInfo2 ParseFunctionFilter(ParseContext context)
            {
                SkipWhiteSpace(context);
                if (context.Current() == ')')
                {
                    return null;
                }
                return ParseFilterExpression(context); ;
            }

        }

        private string JoinNames(IEnumerable<string> names)
        {
            return string.Join('.', names);
        }
        private string ParseName(ParseContext context)
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
            else if (IsNumberStartChar(current))
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

        private bool IsNumberStartChar(char current) => char.IsDigit(current) || current == _numberDecimal || current == _numberNegativeSign || current == _numberPositiveSign;

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
