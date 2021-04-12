using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace YS.Knife.Aop
{
    public class StringResourcesAttribute : DynamicProxyAttribute
    {
        public StringResourcesAttribute() : base(ServiceLifetime.Singleton)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SrAttribute : BaseAopAttribute
    {
        public SrAttribute(string key, string defaultValue)
        {
            this.Key = key;
            this.Value = defaultValue;
        }
        public string Key { get; }

        public string Value { get; }

        public override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var type = typeof(IStringLocalizer<>).MakeGenericType(context.ServiceMethod.DeclaringType);
            var localizer = context.ServiceProvider.GetRequiredService(type) as IStringLocalizer;
            var resourceKey = string.IsNullOrEmpty(Key) ? context.ServiceMethod.Name : Key;
            var localizedString = localizer.GetString(resourceKey);
            var template = localizedString.ResourceNotFound ? this.Value : localizedString.Value;
            context.ReturnValue = FormatTemplate(template, context);
            return context.Break();
        }


        private string FormatTemplate(string template, AspectContext context)
        {
            var factory = context.ServiceProvider.GetRequiredService<ValuesFormatterFactory>();
            var formatter = factory.GetFromTemplete(template);
            var argsValueMap = BuildArgumentMap(context);
            var args = formatter.ValueNames.Select(name => argsValueMap.GetValueOrDefault(name, null));
            return formatter.Format(args.ToArray());
        }

        private Dictionary<string, object> BuildArgumentMap(AspectContext context)
        {
            var dic = new Dictionary<string, object>();
            var allParameters = context.ProxyMethod.GetParameters();
            for (int i = 0; i < allParameters.Length; i++)
            {
                dic.Add(i.ToString(), context.Parameters[i]);
                dic.Add(allParameters[i].Name, context.Parameters[i]);
            }
            return dic;
        }

    }


    [Service(typeof(ValuesFormatterFactory), Lifetime = ServiceLifetime.Singleton)]
    public class ValuesFormatterFactory
    {
        public ValuesFormatter GetFromTemplete(string template)
        {
            return new ValuesFormatter(template);
        }
    }

    public class ValuesFormatter
    {
        private const string NullValue = "(null)";
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
                    int formatDelimiterIndex = FindIndexOfAny(format, FormatDelimiters, openBraceIndex, closeBraceIndex);

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

        public string Format(object[] values)
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

    }
}
