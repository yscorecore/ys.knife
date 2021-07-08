using System;

namespace YS.Knife.Data
{

    class ParseErrors
    {
        public static Exception InvalidText(ParseContext context)
        {
            throw new FilterInfoParseException($"Invalid text near index {context.Index}.");
        }
        public static Exception InvalidFieldNameText(ParseContext context)
        {
            throw new FilterInfoParseException($"Invalid field name near index {context.Index}.");
        }
        public static Exception InvalidFilterType(ParseContext context)
        {
            throw new FilterInfoParseException($"Invalid filter type near index {context.Index}.");
        }
        public static Exception InvalidFilterType(ParseContext context, string code)
        {
            throw new FilterInfoParseException($"Invalid filter type '{code}' near index {context.Index}.");
        }
        public static Exception InvalidValue(ParseContext context)
        {
            throw new FilterInfoParseException($"Invalid value near index {context.Index}.");
        }
        public static Exception InvalidKeywordValue(ParseContext context, string keyword)
        {
            throw new FilterInfoParseException($"Invalid keyword '{keyword}' near index {context.Index}.");
        }
        public static Exception InvalidStringValue(ParseContext context, string str, Exception inner)
        {
            throw new FilterInfoParseException($"Invalid string '{str}' near index {context.Index}.", inner);
        }
        public static Exception InvalidNumberValue(ParseContext context, string str, Exception inner)
        {
            throw new FilterInfoParseException($"Invalid number '{str}' near index {context.Index}.", inner);
        }

        public static Exception MissOpenBracket(ParseContext context)
        {
            throw new FilterInfoParseException($"Invalid expression, missing open bracket near index {context.Index}.");
        }
        public static Exception MissCloseBracket(ParseContext context)
        {
            throw new FilterInfoParseException($"Invalid expression, missing close bracket near index {context.Index}.");
        }

        public static class Select
        {
            public static Exception InvalidText(ParseContext context)
            {
                throw new FilterInfoParseException($"Invalid text near index {context.Index}.");
            }
        }
    }


}
