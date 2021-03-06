﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;


namespace YS.Knife
{
    public static class ServiceProviderExtensions
    {
        public static T GetServiceByKey<T>(this IServiceProvider serviceProvider, string key)
        {
            _ = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            var dict = serviceProvider.GetRequiredService<Dictionary<string, T>>();
            if (dict.TryGetValue(key, out var val))
            {
                return val;
            }
            return default;
        }
    }
}
