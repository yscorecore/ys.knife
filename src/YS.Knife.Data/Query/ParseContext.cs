using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using YS.Knife.Data.Query.Functions;
using YS.Knife.Data.Query.Functions.Collections;

namespace YS.Knife.Data.Query
{

    public class ParseContext
    {
        public static readonly Func<char, bool> IsValidNameFirstChar = ch => char.IsLetter(ch) || ch == '_';
        public static readonly Func<char, bool> IsValidNameChar = ch => char.IsLetterOrDigit(ch) || ch == '_';

        public CultureInfo CurrentCulture { get; }

        public char NumberDecimal { get; } // 小数点
        public char NumberNegativeSign { get; }// 负号
        public char NumberPositiveSign { get; } // 正号
        public char NumberGroupSeparator { get; }// 分组符号

        [Obsolete("should use ctor with culture info")]
        public ParseContext(string text) : this(text, CultureInfo.CurrentCulture)
        {

        }
        public ParseContext(string text, CultureInfo cultureInfo)
        {
            this.Text = text;
            this.TotalLength = text.Length;
            this.CurrentCulture = cultureInfo;
            this.NumberDecimal = cultureInfo.NumberFormat.NumberDecimalSeparator[0];
            this.NumberNegativeSign = cultureInfo.NumberFormat.NegativeSign[0];
            this.NumberPositiveSign = cultureInfo.NumberFormat.PositiveSign[0];
            // this._numberGroupSeparator = cultureInfo.NumberFormat.NumberGroupSeparator[0];
            // default number group separator will conflict with array separator
            // eg. [1,234], so use '_' instead of default number group separator
            this.NumberGroupSeparator = '_';
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


        public (bool, string) TryParseName()
        {
            if (NotEnd() && IsValidNameFirstChar(Current()))
            {
                int startIndex = this.Index;
                Index++;
                while (NotEnd() && IsValidNameChar(Current()))
                {
                    Index++;
                }
                return (true, Text.Substring(startIndex, Index - startIndex));
            }
            else
            {
                return (false, null);
            }
        }
        public (bool, int) TryParseUnsignInt32()
        {
            int startIndex = Index;
            while (NotEnd() && char.IsDigit(Current()))
            {
                Index++;
            }
            if (Index > startIndex)
            {
                string numText = Text.Substring(startIndex, Index - startIndex);
                try
                {
                    return (true, int.Parse(numText));
                }
                catch (Exception ex)
                {
                    throw ParseErrors.InvalidNumberValue(this, numText, ex);
                }
            }
            else
            {
                return (false, 0);
            }

        }
    }

    public static class ParseContextEntensions
    {
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

        internal static readonly Dictionary<string, Operator> FilterTypeCodes =
          typeof(Operator).GetFields(BindingFlags.Public | BindingFlags.Static)
            .SelectMany(p => p.GetCustomAttributes<OperatorCodeAttribute>().Select(c => (c.Code, (Operator)p.GetValue(null))))
            .ToDictionary(p => p.Code, p => p.Item2, StringComparer.InvariantCultureIgnoreCase);

        private static readonly Func<char, bool> IsValidNameFirstChar = ch => char.IsLetter(ch) || ch == '_';
        private static readonly Func<char, bool> IsValidNameChar = ch => char.IsLetterOrDigit(ch) || ch == '_';
        private static readonly Func<char, bool> IsWhiteSpace = ch => ch == ' ' || ch == '\t';
        private static readonly Func<char, bool> IsEscapeChar = ch => ch == '\\';
        private static readonly Func<char, bool> IsOperationChar = ch => ch == '=' || ch == '<' || ch == '>' || ch == '!';

        private static bool IsNumberStartChar(char current, ParseContext context) => char.IsDigit(current) || current == context.NumberDecimal || current == context.NumberNegativeSign || current == context.NumberPositiveSign;


        public static bool SkipWhiteSpace(this ParseContext context)
        {
            while (context.NotEnd())
            {
                if (IsWhiteSpace(context.Text[context.Index]))
                {
                    context.Index++;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        public static void SkipWhiteSpaceAndFirstChar(this ParseContext context, char ch)
        {
            if (context.SkipWhiteSpace())
            {
                if (context.Current() != ch)
                {
                    throw ParseErrors.ExpectedCharNotFound(context, ch);
                }
                else
                {
                    context.Index++;
                }
            }
            else
            {
                throw ParseErrors.ExpectedCharNotFound(context, ch);
            }
        }

        public static ValueInfo ParseValueInfo(this ParseContext context)
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


        public static string ParseName(this ParseContext context)
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
        public static (bool, object) TryParseValue(this ParseContext context)
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
            else if (IsNumberStartChar(current, context))
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


            string ParseStringValue(ParseContext context)
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

            object ParseNumberValue(ParseContext context)
            {
                int startIndex = context.Index;
                bool hasDecimsl = context.Current() == context.NumberDecimal;
                bool hasDigit = char.IsDigit(context.Current());
                // skip first char
                context.Index++;
                while (context.NotEnd())
                {
                    var current = context.Current();
                    if (char.IsDigit(current))
                    {
                        hasDigit = true;
                        context.Index++;
                    }
                    else if (current == context.NumberGroupSeparator)
                    {
                        if (hasDecimsl)
                        {
                            throw ParseErrors.InvalidValue(context);
                        }
                        context.Index++;
                    }
                    else if (current == context.NumberDecimal)
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
                if (!hasDigit)
                {
                    throw ParseErrors.InvalidValue(context);
                }
                var numString = context.Text.Substring(startIndex, context.Index - startIndex);
                try
                {
                    if (hasDecimsl)
                    {
                        return double.Parse(numString.Replace(context.NumberGroupSeparator.ToString(), string.Empty), NumberStyles.Any, context.CurrentCulture);

                    }
                    else
                    {
                        return int.Parse(numString.Replace(context.NumberGroupSeparator.ToString(), string.Empty), NumberStyles.Any, context.CurrentCulture);

                    }

                }
                catch (Exception ex)
                {
                    throw ParseErrors.InvalidNumberValue(context, numString, ex);
                }
            }

            object ParseArrayValue(ParseContext context)
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
            object ParseKeywordValue(ParseContext context)
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

            object ParseValue(ParseContext context, bool parseArray = true)
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
                else if (IsNumberStartChar(current, context))
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
        }

        public static List<ValuePath> ParsePropertyPaths(this ParseContext context)
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
                    var args = IFilterFunction.ParseFunctionArgument(name, context);
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

        public static FilterInfo ParseFilterInfo(this ParseContext context)
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
            FilterInfo ParseCombinFilter(ParseContext context)
            {
                List<FilterInfo> orItems = new List<FilterInfo>();
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
                    var inner = ParseFilterInfo(context);
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
                return orItems.Count > 1 ? new FilterInfo { OpType = CombinSymbol.OrItems, Items = orItems } : orItems.FirstOrDefault();
            }
            CombinSymbol? TryParseOpType(ParseContext context)
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

            FilterInfo ParseSingleItemOne(ParseContext context)
            {
                var leftValue = context.ParseValueInfo();

                var type = ParseType(context);

                var rightValue = context.ParseValueInfo();

                return new FilterInfo()
                {
                    OpType = CombinSymbol.SingleItem,
                    Left = leftValue,
                    Operator = type,
                    Right = rightValue,
                };

            }

            Operator ParseType(ParseContext context)
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
        }

        public static OrderInfo ParseOrderInfo(this ParseContext context)
        {
            OrderInfo orderInfo = new OrderInfo();
            while (context.SkipWhiteSpace())
            {
                var paths = context.ParsePropertyPaths();
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

        public static LimitInfo ParseLimitInfo(this ParseContext context)
        {
            if (context.SkipWhiteSpace())
            {
                var (first, firstNumber) = TryParseUnsignInt32(context);
                if (!first)
                {
                    throw ParseErrors.ParaseLimitNumberError(context);
                }
                if (context.SkipWhiteSpace() && context.Current() == ',')
                {
                    context.Index++;
                    var (second, secondNumber) = TryParseUnsignInt32(context);
                    if (second)
                    {
                        return new LimitInfo { Offset = firstNumber, Limit = secondNumber };
                    }
                    else
                    {
                        return new LimitInfo { Limit = firstNumber };
                    }
                }
                else
                {
                    return new LimitInfo { Limit = firstNumber };
                }
            }
            else
            {
                throw ParseErrors.ParaseLimitNumberError(context);
            }


            (bool, int) TryParseUnsignInt32(ParseContext context)
            {
                int startIndex = context.Index;
                context.SkipWhiteSpace();
                while (context.NotEnd() && char.IsDigit(context.Current()))
                {
                    context.Index++;
                }
                if (context.Index > startIndex)
                {
                    string numText = context.Text.Substring(startIndex, context.Index - startIndex);
                    try
                    {
                        return (true, int.Parse(numText));
                    }
                    catch (Exception ex)
                    {
                        throw ParseErrors.InvalidNumberValue(context, numText, ex);
                    }
                }
                else
                {
                    return (false, 0);
                }

            }
        }

        public static SelectInfo ParseSelectInfo(this ParseContext context)
        {
            SelectInfo selectInfo = new SelectInfo
            {
                Items = new List<SelectItem>()
            };
            while (context.SkipWhiteSpace())
            {
                var (found, name) = context.TryParseName();
                if (found)
                {
                    selectInfo.Items.Add(ParseSelectItem(name, context));
                    if (context.SkipWhiteSpace() && context.Current() == ',')
                    {
                        context.Index++;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            return selectInfo;
            SelectItem ParseSelectItem(string name, ParseContext context)
            {
                SelectItem item = new SelectItem { Name = name };
                if (context.SkipWhiteSpace() && context.Current() == '{')
                {
                    // parse collection infos
                    ParseCollectionInfos2(item, context);
                }
                if (context.SkipWhiteSpace() && context.Current() == '(')
                {
                    // parse sub items
                    context.Index++;
                    item.SubItems = ParseSelectInfo(context).Items;
                    if (context.SkipWhiteSpace() == false || context.Current() != ')')
                    {
                        throw ParseErrors.MissCloseBracket(context);
                    }
                    else
                    {
                        context.Index++;
                    }
                }
                return item;
            }
            void ParseCollectionInfos2(SelectItem selectItem, ParseContext context)
            {
                context.SkipWhiteSpaceAndFirstChar('{');
                var set = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
                do
                {
                    if (!context.SkipWhiteSpace())
                    {
                        break;
                    }
                    if (context.Current() == '}')
                    {
                        break;
                    }
                    var valueInfo = context.ParseValueInfo();

                    if (valueInfo.IsConstant || valueInfo.NavigatePaths.Count != 1 || valueInfo.NavigatePaths[0].IsFunction == false)
                    {
                        throw ParseErrors.OnlySupportCollectionFunctionInCurlyBracket(context);
                    }
                    var functionName = valueInfo.NavigatePaths[0].Name;
                    if (set.Contains(functionName))
                    {
                        throw ParseErrors.DuplicateCollectionFunctionInCurlyBracket(context, functionName);
                    }
                    else
                    {
                        set.Add(functionName);
                    }
                    SetCollectionFilterValue(context, selectItem, valueInfo.NavigatePaths[0]);
                    if (!context.SkipWhiteSpace())
                    {
                        break;
                    }
                    if (context.Current() == ',')
                    {
                        context.Index++;
                    }
                    else if (context.Current() == '}')
                    {
                        break;
                    }
                    else
                    {
                        throw ParseErrors.InvalidText(context);
                    }
                }
                while (true);
                context.SkipWhiteSpaceAndFirstChar('}');

            }
            void SetCollectionFilterValue(ParseContext context, SelectItem selectItem, ValuePath valuePath)
            {
                if (nameof(Where).Equals(valuePath.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    selectItem.CollectionFilter = valuePath.FunctionArgs.Cast<FilterInfo>().FirstOrDefault();
                }
                else if (nameof(OrderBy).Equals(valuePath.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    selectItem.CollectionOrder = valuePath.FunctionArgs.Cast<OrderInfo>().FirstOrDefault();
                }
                else if (nameof(Limit).Equals(valuePath.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    selectItem.CollectionLimit = valuePath.FunctionArgs.Cast<LimitInfo>().FirstOrDefault();
                }
                else
                {
                    throw ParseErrors.OnlySupportCollectionFunctionInCurlyBracket(context);
                }

            }
        }
    }
}
