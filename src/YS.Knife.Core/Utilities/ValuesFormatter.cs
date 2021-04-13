using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace YS.Knife
{
    public class ValuesFormatter
    {
        private const string NullValue = "[null]";
        private static readonly char[] FormatDelimiters = { ',', ':' };
        private readonly string _format;
        private readonly List<string> _valueNames = new List<string>();

        public ValuesFormatter(string format)
        {
            OriginalFormat = format ?? throw new ArgumentNullException(nameof(format));

            var sb = new StringBuilder();
            int scanIndex = 0;
            int endIndex = format.Length;

            while (scanIndex < endIndex)
            {
                int openBraceIndex = FindBraceIndex(format, '{', scanIndex, endIndex);
                int closeBraceIndex = FindBraceIndex(format, '}', openBraceIndex, endIndex);

                if (closeBraceIndex == endIndex)
                {
                    sb.Append(format, scanIndex, endIndex - scanIndex);
                    scanIndex = endIndex;
                }
                else
                {
                    // Format item syntax : { index[,alignment][ :formatString] }.
                    int formatDelimiterIndex =
                        FindIndexOfAny(format, FormatDelimiters, openBraceIndex, closeBraceIndex);

                    sb.Append(format, scanIndex, openBraceIndex - scanIndex + 1);
                    sb.Append(_valueNames.Count.ToString(CultureInfo.InvariantCulture));
                    _valueNames.Add(format.Substring(openBraceIndex + 1, formatDelimiterIndex - openBraceIndex - 1));
                    sb.Append(format, formatDelimiterIndex, closeBraceIndex - formatDelimiterIndex + 1);

                    scanIndex = closeBraceIndex + 1;
                }
            }

            _format = sb.ToString();
        }

        public string OriginalFormat { get; private set; }
        public List<string> ValueNames => _valueNames;

        private static int FindBraceIndex(string format, char brace, int startIndex, int endIndex)
        {
            // Example: {{prefix{{{Argument}}}suffix}}.
            int braceIndex = endIndex;
            int scanIndex = startIndex;
            int braceOccurrenceCount = 0;

            while (scanIndex < endIndex)
            {
                if (braceOccurrenceCount > 0 && format[scanIndex] != brace)
                {
                    if (braceOccurrenceCount % 2 == 0)
                    {
                        // Even number of '{' or '}' found. Proceed search with next occurrence of '{' or '}'.
                        braceOccurrenceCount = 0;
                        braceIndex = endIndex;
                    }
                    else
                    {
                        // An unescaped '{' or '}' found.
                        break;
                    }
                }
                else if (format[scanIndex] == brace)
                {
                    if (brace == '}')
                    {
                        if (braceOccurrenceCount == 0)
                        {
                            // For '}' pick the first occurrence.
                            braceIndex = scanIndex;
                        }
                    }
                    else
                    {
                        // For '{' pick the last occurrence.
                        braceIndex = scanIndex;
                    }

                    braceOccurrenceCount++;
                }

                scanIndex++;
            }

            return braceIndex;
        }

        private static int FindIndexOfAny(string format, char[] chars, int startIndex, int endIndex)
        {
            int findIndex = format.IndexOfAny(chars, startIndex, endIndex - startIndex);
            return findIndex == -1 ? endIndex : findIndex;
        }

        public string Format(IList<KeyValuePair<string, object>> args)
        {
            var kwargs = new Dictionary<string, object>();
            if (args != null && args.Count > 0)
            {
                for (int i = 0; i < args.Count; i++)
                {
                    var kv = args[i];
                    if (kv.Key != null)
                    {
                        kwargs[kv.Key] = kv.Value;
                        kwargs[i.ToString()] = kv.Value;
                    }
                }
            }

            return Format(kwargs);
        }

        public string Format(object[] args, IDictionary<string, object> kwargs)
        {
            if (args == null || args.Length == 0)
            {
                return Format(kwargs);
            }

            var dic = kwargs != null ? new Dictionary<string, object>(kwargs) : new Dictionary<string, object>();
            for (int i = 0; i < args.Length; i++)
            {
                dic[i.ToString()] = args[i];
            }

            return Format(dic);
        }

        private string Format(IDictionary<string, object> kwargs)
        {
            var values = this.ValueNames.Select(p =>
            {
                if (kwargs != null && kwargs.TryGetValue(p, out var value))
                {
                    return value;
                }

                return null;
            }).ToArray();
            return Format(values);
        }

        private string Format(object[] values)
        {
            object[] formatedValues = new object[values == null ? 0 : values.Length];
            if (values != null)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    formatedValues[i] = FormatArgument(values[i]);
                }
            }

            return string.Format(CultureInfo.InvariantCulture, _format, formatedValues ?? Array.Empty<object>());
        }

        public IEnumerable<KeyValuePair<string, object>> GetValues(object[] values)
        {
            var valueArray = new KeyValuePair<string, object>[values.Length];
            for (int index = 0; index < _valueNames.Count; ++index)
            {
                valueArray[index] = new KeyValuePair<string, object>(_valueNames[index], values[index]);
            }

            return valueArray;
        }

        private object FormatArgument(object value)
        {
            if (value == null)
            {
                return NullValue;
            }

            // since 'string' implements IEnumerable, special case it
            if (value is string)
            {
                return value;
            }

            // if the value implements IEnumerable, build a comma separated string.
            if (value is IEnumerable enumerable)
            {
                return string.Join(", ", enumerable.Cast<object>().Select(o => o ?? NullValue));
            }

            return value;
        }

        private static readonly LocalCache<string, ValuesFormatter> _localCache =
            new LocalCache<string, ValuesFormatter>();

        public static ValuesFormatter FromText(string text)
        {
            _ = text ?? throw new ArgumentNullException(nameof(text));
            return _localCache.Get(text, t => new ValuesFormatter(t));
        }
    }
}
