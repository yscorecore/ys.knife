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

        internal static readonly Dictionary<string, Operator> FilterTypeCodes =
            FilterInfo.FilterTypeNameMapper.Select(p => Tuple.Create(p.Value, p.Key))
            .Concat(new[] {
                Tuple.Create("=",Operator.Equals),
                Tuple.Create("<>",Operator.NotEquals)

            }).ToDictionary(p => p.Item1, p => p.Item2);
        internal static readonly Dictionary<string, object> KeyWordValues = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase)
        {
            ["true"] = true,
            ["false"] = false,
            ["null"] = null
        };
        internal static readonly Dictionary<string, CombinSymbol> OpTypeCodes = new Dictionary<string, CombinSymbol>(StringComparer.InvariantCultureIgnoreCase)
        {
            [FilterInfo.Operator_And] = CombinSymbol.AndItems,
            [FilterInfo.Operator_Or] = CombinSymbol.OrItems
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

        public List<ValuePath> ParsePaths(string text)
        {
            var context = new ParseContext(text);
            SkipWhiteSpace(context);
            var paths = ParsePropertyPaths(context);
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return paths;
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
            CombinSymbol lastOpType = CombinSymbol.OrItems;
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

                if (lastOpType == CombinSymbol.OrItems || orItems.Count == 0)
                {
                    orItems.Add(inner);
                }
                else
                {
                    orItems[^1] = orItems[^1].AndAlso(inner);
                }

                CombinSymbol? opType = TryParseOpType(context);

                if (opType == null)
                {
                    break;
                }
                else
                {
                    lastOpType = opType.Value;
                }
            }
            return orItems.Count > 1 ? new FilterInfo2 { OpType = CombinSymbol.OrItems, Items = orItems } : orItems.FirstOrDefault();
        }
        private CombinSymbol? TryParseOpType(ParseContext context)
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
            var leftValue = ParseValueInfo(context);

            var type = ParseType(context);

            var rightValue = ParseValueInfo(context);

            return new FilterInfo2()
            {
                OpType = CombinSymbol.SingleItem,
                Left = leftValue,
                Operator = type,
                Right = rightValue,
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

        private FilterValue ParseValueInfo(ParseContext context)
        {
            SkipWhiteSpace(context);

            var (isValue, value) = TryParseValue(context);
            if (isValue)
            {
                return new FilterValue() { IsConstant = true, ConstantValue = value };
            }

            var propertyPaths = ParsePropertyPaths(context);

            return new FilterValue { IsConstant = false, NavigatePaths = propertyPaths };
        }

        private List<ValuePath> ParsePropertyPaths(ParseContext context)
        {
            List<ValuePath> names = new List<ValuePath>();
            while (context.NotEnd())
            {
                var name = ParseName(context);
                SkipWhiteSpace(context);
                if (context.End())
                {
                    names.Add(new ValuePath { Name = name });
                    break;
                }
                else if (context.Current() == '.')
                {
                    // a.b
                    names.Add(new ValuePath { Name = name });
                    context.Index++;

                }

                else if (context.Current() == '(')
                {
                    var (args, subFilter) = ParseFunctionBody2(context);
                    var nameInfo = new ValuePath { Name = name, IsFunction = true, FunctionArgs = args, FunctionFilter = subFilter };
                    SkipWhiteSpace(context);
                    names.Add(nameInfo);
                    if (context.NotEnd())
                    {
                        if (context.Current() == '.')
                        {
                            context.Index++;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    names.Add(new ValuePath { Name = name });
                    break;
                }

            }
            return names;
        }
        private (List<FilterValue> Args, FilterInfo2 SubFilter) ParseFunctionBody2(ParseContext context)
        {
            context.Index++;
            List<FilterValue> args = ParseFunctionArguments(context);
            FilterInfo2 filterInfo = ParseFunctionFilter(context);

            SkipCloseBracket(context);
            return (args, filterInfo);
            FilterValue NameChainToValue(string nameChain)
            {
                var items = nameChain.Split('.');
                if (items.Length == 1 && KeyWordValues.ContainsKey(items[0]))
                {
                    return new FilterValue { IsConstant = true, ConstantValue = KeyWordValues[items[0]] };
                }
                else
                {
                    return new FilterValue { IsConstant = false, NavigatePaths = items.Select(p => new ValuePath { Name = p }).ToList() };
                }
            }
            List<FilterValue> ParseFunctionArguments(ParseContext context)
            {
                List<FilterValue> datas = new List<FilterValue>();
                while (context.NotEnd())
                {
                    SkipWhiteSpace(context);
                    if (IsNumberStartChar(context.Current()))
                    {
                        //number
                        datas.Add(new FilterValue { IsConstant = true, ConstantValue = ParseNumberValue(context) });
                    }
                    else if (context.Current() == '\"')
                    {
                        //string
                        datas.Add(new FilterValue { IsConstant = true, ConstantValue = ParseStringValue(context) });
                    }
                    else if (IsValidNameFirstChar(context.Current()))
                    {
                        var originStartIndex = context.Index;
                        var nameChain = ParseNameChain(context);
                        var values = NameChainToValue(nameChain);
                        SkipWhiteSpace(context);
                        if (context.Current() == ',')
                        {
                            datas.Add(values);
                        }
                        else if (context.Current() == ')')
                        {
                            datas.Add(values);
                            break;
                        }
                        else if (context.Current() == '(')
                        {
                            //nested function
                            if (values.IsConstant)
                            {
                                throw ParseErrors.InvalidText(context);
                            }
                            else
                            {
                                var nestedFunction = values.NavigatePaths.Last();
                                nestedFunction.IsFunction = true;
                                nestedFunction.FunctionArgs = ParseFunctionArguments(context);
                                nestedFunction.FunctionFilter = ParseFunctionFilter(context);
                                SkipCloseBracket(context);
                            }

                        }
                        else
                        {
                            context.Index = originStartIndex;
                            break;
                        }
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

        private Operator ParseType(ParseContext context)
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
                if (FilterTypeCodes.TryGetValue(opCode.ToLowerInvariant(), out Operator filterType))
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
                if (FilterTypeCodes.TryGetValue(opCode, out Operator filterType))
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
        private (bool, object) TryParseValue(ParseContext context)
        {
            var originIndex = context.Index;
            SkipWhiteSpace(context);
            var current = context.Current();
            if (current == '\"')
            {
                //string
                return (true, ParseStringValue(context));
            }
            else if (char.IsLetter(current))
            {
                //keyword eg
                string name = ParseName(context);
                if (KeyWordValues.TryGetValue(name, out var val))
                {
                    return (true, val);
                }
                else
                {
                    context.Index = originIndex;
                    return (false, null);
                }
            }
            else if (IsNumberStartChar(current))
            {
                //number
                return (true, ParseNumberValue(context));
            }
            else if (current == '[')
            {
                return (true, ParseArrayValue(context));
                //array
            }
            return (false, null);

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
            public bool End()
            {
                return Index >= TotalLength;
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
