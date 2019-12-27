
using System.Collections.Generic;

namespace System.Collections.Generic
{
    public class InjectionDictionary<T> : Dictionary<string, T>
    {
        public InjectionDictionary(IEnumerable<T> instances) : base(instances.ToServiceDictionary())
        {
        }
    }
}
