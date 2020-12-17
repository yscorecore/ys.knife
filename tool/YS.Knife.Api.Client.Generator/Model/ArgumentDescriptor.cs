using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace YS.Knife.Api.Client.Generator.Model
{
    public class ArgumentDescriptor
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public ArgumentSource Source { get; private set; }
        public static ArgumentDescriptor FromParameter(ParameterInfo parameterInfo)
        {
            bool IsSourceAttribute(Attribute attribute)
            {
                var attrType = attribute.GetType();
                return attrType.Namespace == typeof(FromQueryAttribute).Namespace && attrType.Name.StartsWith("From");
            }
            var source = parameterInfo.GetCustomAttributes().Where(IsSourceAttribute).Select(p => p.GetType().Name.Replace(nameof(Attribute), string.Empty)).FirstOrDefault();

            var argumentSource = source == null ? ArgumentSource.Unknown : (ArgumentSource)Enum.Parse(typeof(ArgumentSource), source);
            return new ArgumentDescriptor
            {
                Name = parameterInfo.Name,
                Type = parameterInfo.ParameterType,
                Source = source == null ? ArgumentSource.Unknown : (ArgumentSource)Enum.Parse(typeof(ArgumentSource), source)
            };
        }
    }

}
