using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace YS.Knife
{
    public static class Should
    {
        [DebuggerHidden]
        public static void NotNull(object obj, Func<Exception> func)
        {
            _ = func ?? throw new ArgumentNullException(nameof(func));
            if (obj is null)
            {
                var ex = func();
                if (ex != null) throw func();
            }
        }

        [DebuggerHidden]
        public static void NotEmpty(string str, Func<Exception> func)
        {
            _ = func ?? throw new ArgumentNullException(nameof(func));
            if (string.IsNullOrEmpty(str))
            {
                var ex = func();
                if (ex != null) throw func();
            }
        }

        [DebuggerHidden]
        public static void NotBlank(string str, Func<Exception> func)
        {
            _ = func ?? throw new ArgumentNullException(nameof(func));
            if (string.IsNullOrWhiteSpace(str))
            {
                var ex = func();
                if (ex != null) throw func();
            }
        }

        [DebuggerHidden]
        public static void NotEmpty<T>(IEnumerable<T> collection, Func<Exception> func)
        {
            _ = func ?? throw new ArgumentNullException(nameof(func));
            if (collection is null || !collection.Any())
            {
                var ex = func();
                if (ex != null) throw func();
            }
        }

        [DebuggerHidden]
        public static void BeTrue(bool condition, Func<Exception> func)
        {
            _ = func ?? throw new ArgumentNullException(nameof(func));
            if (!condition)
            {
                var ex = func();
                if (ex != null) throw func();
            }
        }

        [DebuggerHidden]
        public static void BeFalse(bool condition, Func<Exception> func)
        {
            _ = func ?? throw new ArgumentNullException(nameof(func));
            if (condition)
            {
                var ex = func();
                if (ex != null) throw func();
            }
        }
    }
}
