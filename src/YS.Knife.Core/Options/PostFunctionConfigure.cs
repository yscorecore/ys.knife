using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;

namespace YS.Knife.Options
{
    [ServiceClass(typeof(IPostConfigureOptions<>), ServiceLifetime.Singleton)]
    public class PostFunctionConfigure<TOptions> : IPostConfigureOptions<TOptions>
        where TOptions : class
    {
        static Regex functionPattern = new Regex(@"\$\{\{(?<func>\w+)\((?<args>.*?)\)\}\}");

        public PostFunctionConfigure(ILogger<PostFunctionConfigure<TOptions>> logger, IDictionary<string, IOptionsPostFunction> functions)
        {
            this.logger = logger;
            this.functions = functions;
        }
        readonly IDictionary<string, IOptionsPostFunction> functions;
        readonly ILogger logger;
        public void PostConfigure(string name, TOptions options)
        {
            ConvertOptions(options);
        }

        private void ConvertOptions(object options)
        {
            if (options == null) return;
            foreach (var prop in options.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var typecode = Type.GetTypeCode(prop.PropertyType);
                var propValue = prop.GetValue(options);
                if (propValue == null) continue;
                if (typecode == TypeCode.String)
                {
                    if (prop.CanRead && prop.CanWrite)
                    {
                        var strValue = propValue as string;
                        if (string.IsNullOrEmpty(strValue)) continue;
                        var match = functionPattern.Match(strValue);
                        if (!match.Success) continue;
                        var replaced = ConvertFunctionResult(strValue);
                        if (strValue != replaced)
                        {
                            prop.SetValue(options, replaced);
                        }
                    }
                }
                else if (typecode == TypeCode.Object)
                {
                    if (typeof(IDictionary).IsAssignableFrom(prop.PropertyType))
                    {
                        foreach (var value in (propValue as IDictionary).Values)
                        {
                            ConvertOptions(value);
                        }
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                    {
                        foreach (var value in propValue as IEnumerable)
                        {
                            ConvertOptions(value);
                        }
                    }
                    else
                    {
                        ConvertOptions(propValue);
                    }
                }
            }
        }

        private string ConvertFunctionResult(string input)
        {
            return functionPattern.Replace(input, (match) =>
            {

                var func = match.Groups["func"].Value;
                var args = match.Groups["args"].Value;
                if (functions.TryGetValue(func, out var handler))
                {
                    return TryInvodeFunction(handler, func, match.Value, args);
                }
                else
                {
                    logger.LogWarning("Options function @{function} not found.", func);
                    return match.Value;
                }
            });

        }
        private string TryInvodeFunction(IOptionsPostFunction function, string functionName, string fullFunctionDesc, string args)
        {
            try
            {
                return function.Invoke(args);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Invoke function @{function} error.", functionName);
                return fullFunctionDesc;
            }
        }

    }



}
