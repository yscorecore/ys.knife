using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YS.Knife.Generator.UnitTest
{
    static class StringExtensions
    {
        public static IEnumerable<string> SplitCode(this string text)
        {

            using (var reader = new StringReader(text.Trim()))
            {
                while (true)
                {
                    string line = reader.ReadLine();
                    if (line != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            yield return line;

                        }

                    }
                    else
                    {
                        yield break;
                    }
                }
            }
        }
        public static string NormalizeCode(this string input)
        {
            return input.Trim().Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
        }
    }
}
