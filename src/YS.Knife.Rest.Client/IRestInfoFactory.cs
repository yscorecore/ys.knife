using System;

namespace YS.Knife.Rest.Client
{
    public interface IRestInfoFactory
    {
        RestInfo GetRestInfo(string serviceName);
    }
    public static class RestInfoFactoryExtensions
    {
        public static RestInfo GetRestInfo(this IRestInfoFactory factory, Type callerType)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            _ = callerType ?? throw new ArgumentNullException(nameof(callerType));
            return factory.GetRestInfo(callerType.FullName);
        }
        public static RestInfo GetRestInfo<T>(this IRestInfoFactory factory)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            return factory.GetRestInfo(typeof(T));
        }
    }
}
