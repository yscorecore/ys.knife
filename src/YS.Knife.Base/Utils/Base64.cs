using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace YS.Knife.Data
{
    public class Base64
    {
        const char Base64Padding = '=';
        const string Base64CharTable = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        static readonly HashSet<char> base64Table = Base64CharTable.ToCharArray().ToHashSet();
        public static bool IsBase64String(string value)
        {
            if (value is null)
            {
                return false;
            }

            value = value.Replace("\r", string.Empty).Replace("\n", string.Empty);

            if (value.Length == 0 || (value.Length % 4) != 0)
            {
                return false;
            }

            var lengthNoPadding = value.Length;
            value = value.TrimEnd(Base64Padding);
            var lengthPadding = value.Length;

            if ((lengthNoPadding - lengthPadding) > 2)
            {
                return false;
            }

            foreach (char c in value)
            {
                if (!base64Table.Contains(c))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
