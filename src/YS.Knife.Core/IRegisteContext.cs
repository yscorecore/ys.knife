using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace YS.Knife
{
    public interface IRegisteContext
    {
        IConfiguration Configuration { get; }
        ILogger Logger { get; set; }
        Func<Type, bool> TypeFilter { get; }
    }

    public static class RegisteContextExtensions
    {
        public static bool HasFiltered(this IRegisteContext context, Type type)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (context.TypeFilter == null) return false;
            return context.TypeFilter(type);
        }
    }
}
