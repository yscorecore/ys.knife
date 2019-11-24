using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class ServiceProviderExtentions
    {
        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }
    }
}
