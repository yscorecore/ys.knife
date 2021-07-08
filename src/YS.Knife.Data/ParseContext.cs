using System;
using System.Diagnostics;

namespace YS.Knife.Data
{

    class ParseContext
    {
        static readonly Func<char, bool> IsValidNameFirstChar = ch => char.IsLetter(ch) || ch == '_';
        static readonly Func<char, bool> IsValidNameChar = ch => char.IsLetterOrDigit(ch) || ch == '_';
        static readonly Func<char, bool> IsWhiteSpace = ch => ch == ' ' || ch == '\t';
        public ParseContext(string text)
        {
            this.Text = text;
            this.TotalLength = text.Length;
        }
        public char Current()
        {
            if (Index >= TotalLength)
            {
                throw ParseErrors.InvalidText(this);
            }
            return Text[Index];
        }
        public bool NotEnd()
        {
            return Index < TotalLength;
        }
        public bool End()
        {
            return Index >= TotalLength;
        }

        public bool SkipWhiteSpace()
        {
            while (NotEnd())
            {
                if (IsWhiteSpace(Text[Index]))
                {
                    Index++;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }




        public string Text;
        public int TotalLength;
        public int Index;


        public (bool, string) TryParseName()
        {
            if (NotEnd() && IsValidNameFirstChar(Current()))
            {
                int startIndex = this.Index;
                Index++;
                while (NotEnd() && IsValidNameChar(Current()))
                {
                    Index++;
                }
                return (true, Text.Substring(startIndex, Index - startIndex));
            }
            else
            {
                return (false, null);
            }
        }
    }


}
