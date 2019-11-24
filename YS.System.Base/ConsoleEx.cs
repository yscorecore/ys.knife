using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace System
{
    public static class ConsoleEx
    {
        public static void Pause(string text = "Press any key to continue...",
              ConsoleColor color = ConsoleColor.Yellow)
        {
            WriteLineColor(color, text);
            Console.ReadKey(true);
        }
        public static char PromptKey(string prompt, params char[] allowedKeys)
        {
            if (allowedKeys == null)
            {
                throw new ArgumentNullException("allowedKeys");
            }
            char keyChar;
            bool validKey;
            var allowedString = ToDelimitedList(allowedKeys);
            do
            {
                Console.Write("{0} ({1}) ", prompt, allowedString);
                keyChar = ToLower(Console.ReadKey(false).KeyChar);
                validKey = Contains(allowedKeys, keyChar);
                if (!validKey)
                {
                    Console.WriteLine("\r\n\"{0}\" is not a valid choice, valid keys are \"{1}\"", keyChar, allowedString);
                }
                else
                {
                    Console.WriteLine();
                }
            }
            while (!validKey);

            return keyChar;
        }
        private static char ToLower(char keyChar)
        {
            return keyChar.ToString().ToLowerInvariant()[0];
        }
        private static string ToDelimitedList(char[] allowedKeys)
        {
            var sb = new StringBuilder();

            for (var index = 0; index < allowedKeys.Length; index++)
            {
                var allowedKey = allowedKeys[index];
                sb.Append(allowedKey);
                if (index + 1 < allowedKeys.Length)
                {
                    sb.Append(System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                }
            }
            return sb.ToString();
        }
        private static bool Contains(char[] allowedKeys, char input)
        {
            foreach (char ch in allowedKeys)
            {
                if (ch.Equals(input))
                {
                    return true;
                }
            }
            return false;
        }
        #region 批量生产的代码
        public static void WriteColor(ConsoleColor color, bool value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(value); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, char value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(value); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, char[] buffer) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(buffer); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, decimal value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(value); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, double value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(value); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, float value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(value); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, int value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(value); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, long value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(value); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, object value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(value); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, string value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(value); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, uint value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(value); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, ulong value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(value); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, string format, object arg0) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(format, arg0); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, string format, params object[] arg) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(format, arg); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, char[] buffer, int index, int count) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(buffer, index, count); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, string format, object arg0, object arg1) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(format, arg0, arg1); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, string format, object arg0, object arg1, object arg2) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(format, arg0, arg1, arg2); Console.ForegroundColor = saveColor; }
        public static void WriteColor(ConsoleColor color, string format, object arg0, object arg1, object arg2, object arg3) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.Write(format, arg0, arg1, arg2, arg3); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, bool value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(value); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, char value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(value); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, char[] buffer) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(buffer); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, decimal value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(value); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, double value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(value); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, float value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(value); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, int value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(value); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, long value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(value); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, object value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(value); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, string value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(value); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, uint value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(value); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, ulong value) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(value); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, string format, object arg0) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(format, arg0); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, string format, params object[] arg) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(format, arg); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, char[] buffer, int index, int count) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(buffer, index, count); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, string format, object arg0, object arg1) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(format, arg0, arg1); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, string format, object arg0, object arg1, object arg2) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(format, arg0, arg1, arg2); Console.ForegroundColor = saveColor; }
        public static void WriteLineColor(ConsoleColor color, string format, object arg0, object arg1, object arg2, object arg3) { var saveColor = Console.ForegroundColor; Console.ForegroundColor = color; Console.WriteLine(format, arg0, arg1, arg2, arg3); Console.ForegroundColor = saveColor; }

        #endregion
    }

    public class OnceConsoleForeColor : IDisposable
    {
        private static object locker = new object();
        private ConsoleColor bkColor;
        public OnceConsoleForeColor(ConsoleColor consoleColor)
        {
            Monitor.Enter(locker);
            this.bkColor = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
        }

        public void Dispose()
        {
            Console.ForegroundColor = bkColor;
            Monitor.Exit(locker);
        }

    }
}
