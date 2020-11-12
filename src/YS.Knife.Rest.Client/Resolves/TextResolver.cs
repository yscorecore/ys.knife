using System;

namespace YS.Knife.Rest.Client.Resolves
{
    public class TextResolver : IEntityResolver
    {
        public T Resolve<T>(string content)
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)content;
            }
            throw new NotSupportedException();
        }
    }
}
