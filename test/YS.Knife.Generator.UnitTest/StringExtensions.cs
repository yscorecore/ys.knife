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
        public static string NormalizeCode(this string input)
        {
            return input.Trim().Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
        }
    }
}
