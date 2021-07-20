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


        public List<ValuePath> ParsePaths(string text)
        {
            var context = new ParseContext(text);
            context.SkipWhiteSpace();
            var paths = context.ParsePropertyPaths();
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
            var leftValue = context.ParseValueInfo();

            var type = ParseType(context);

            var rightValue = context.ParseValueInfo();

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

       

       
      
       

        private string JoinNames(IEnumerable<string> names)
        {
            return string.Join('.', names);
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
       



    }

}
