using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YS.Knife.Data.Filter.Functions;

namespace YS.Knife.Data
{
    internal partial class FilterInfoParser2
    {

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
        public FilterInfo2 ParseFilter(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return null; }
            var context = new ParseContext(text);
            var filterInfo = ParseFilterExpression(context);
            context.SkipWhiteSpace();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return filterInfo;
        }

        public OrderInfo ParseOrder(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;
            var context = new ParseContext(text);
            OrderInfo orderInfo = ParseOrderInfo(context);
            context.SkipWhiteSpace();
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return orderInfo;
        }
        private OrderInfo ParseOrderInfo(ParseContext context)
        {
            OrderInfo orderInfo = new OrderInfo();
            while (context.SkipWhiteSpace())
            {
                var paths = this.ParsePropertyPaths(context);
                orderInfo.Add(OrderItem.FromValuePaths(paths));

                if (context.SkipWhiteSpace() && context.Current() == ',')
                {
                    context.Index++;
                }
                else
                {
                    break;
                }
            }
            return orderInfo.HasItems() ? orderInfo : null;
        }


        public List<ValuePath> ParsePaths(string text)
        {
            var context = new ParseContext(text);
            context.SkipWhiteSpace();
            var paths = ParsePropertyPaths(context);
            if (context.NotEnd())
            {
                throw ParseErrors.InvalidText(context);
            }
            return paths;
        }


        internal FilterInfo2 ParseFilterExpression(ParseContext context)
        {
            context.SkipWhiteSpace();
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
                context.SkipWhiteSpace();
                if (context.Current() != '(')
                {
                    throw ParseErrors.MissOpenBracket(context);
                }
                context.Index++;
                context.SkipWhiteSpace();
                var inner = ParseFilterExpression(context);
                context.SkipWhiteSpace();
                if (context.End() || context.Current() != ')')
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

            context.SkipWhiteSpace();
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
        private void SkipCloseBracket(ParseContext context)
        {
            context.SkipWhiteSpace();
            if (context.End() || context.Current() != ')')
            {
                throw ParseErrors.MissCloseBracket(context);
            }
            else
            {
                context.Index++;
            }
        }

        internal ValueInfo ParseValueInfo(ParseContext context)
        {
            context.SkipWhiteSpace();

            var (isValue, value) = TryParseValue(context);
            if (isValue)
            {
                return new ValueInfo() { IsConstant = true, ConstantValue = value };
            }

            var propertyPaths = ParsePropertyPaths(context);

            return new ValueInfo { IsConstant = false, NavigatePaths = propertyPaths };
        }

        private List<ValuePath> ParsePropertyPaths(ParseContext context)
        {
            List<ValuePath> names = new List<ValuePath>();
            while (context.NotEnd())
            {
                var name = ParseName(context);
                context.SkipWhiteSpace();
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
                    var args = ParseFunctionArguments2(name, context);
                    var nameInfo = new ValuePath { Name = name, IsFunction = true, FunctionArgs = args };
                    context.SkipWhiteSpace();
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
        private object[] ParseFunctionArguments2(string name, ParseContext context)
        {
            //skip open bracket
            context.Index++;

            var arguments = IFilterFunction.ParseFunctionArgument(name, context);
            SkipCloseBracket(context);
            return arguments;


        }
        private (List<ValueInfo> Args, FilterInfo2 SubFilter) ParseFunctionArguments(string name, ParseContext context)
        {
            context.Index++;
            List<ValueInfo> args = ParseFunctionArguments(context);
            FilterInfo2 filterInfo = ParseFunctionFilter(context);

            SkipCloseBracket(context);
            return (args, filterInfo);

            List<ValueInfo> ParseFunctionArguments(ParseContext context)
            {
                List<ValueInfo> arguments = new List<ValueInfo>();
                if (context.SkipWhiteSpace())
                {
                    if (context.Current() == '(')
                    {
                        return null;
                    }
                    if (context.Current() == ')')
                    {
                        return null;
                    }
                }
                while (context.SkipWhiteSpace())
                {

                    var originStartIndex = context.Index;
                    var valueInfo = ParseValueInfo(context);

                    if (context.SkipWhiteSpace())
                    {
                        if (context.Current() == ',')
                        {
                            arguments.Add(valueInfo);
                            context.Index++;
                        }
                        else if (context.Current() == ')')
                        {
                            arguments.Add(valueInfo);
                            break;
                        }
                        else
                        {
                            //reset index
                            context.Index = originStartIndex;
                            break;
                        }
                    }
                    else
                    {
                        throw ParseErrors.InvalidText(context);
                    }
                }
                return arguments.Any() ? arguments : null;
            }
            FilterInfo2 ParseFunctionFilter(ParseContext context)
            {
                context.SkipWhiteSpace();
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
            context.SkipWhiteSpace();
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
            context.SkipWhiteSpace();
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
            context.SkipWhiteSpace();
            if (context.End())
            {
                context.Index = originIndex;
                return (false, null);
            }
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
            context.SkipWhiteSpace();
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
                context.SkipWhiteSpace();
                var current = context.Current();
                if (current == ']')
                {
                    context.Index++;
                    break;
                }
                datas.Add(ParseValue(context, false));
                context.SkipWhiteSpace();
                if (context.Current() == ',')
                {
                    context.Index++;
                }

            }
            return datas.ToArray();
        }
    }

}
