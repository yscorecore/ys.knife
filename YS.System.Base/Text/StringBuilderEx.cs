using System;
using System.Collections.Generic;
using System.Text;

namespace System.Text
{
    public static class StringBuilderEx
    {
        public static void AppendFormatLine(this StringBuilder stringBuilder,string format,object arg0)
        {
            stringBuilder.AppendFormat(format,arg0);
            stringBuilder.AppendLine();
        }
        public static void AppendFormatLine(this StringBuilder stringBuilder,string format,params object[] args)
        {
            stringBuilder.AppendFormat(format,args);
            stringBuilder.AppendLine();
        }
        public static void AppendFormatLine(this StringBuilder stringBuilder,string format,object arg0,object arg1)
        {
            stringBuilder.AppendFormat(format,arg0,arg1);
            stringBuilder.AppendLine();
        }
        public static void AppendFormatLine(this StringBuilder stringBuilder,IFormatProvider provider,string format,params object[] args)
        {
            stringBuilder.AppendFormat(format,provider,args);
            stringBuilder.AppendLine();
        }
        public static void AppendFormatLine(this StringBuilder stringBuilder,string format,object arg0,object arg1,object arg2)
        {
            stringBuilder.AppendFormat(format,arg0,arg1,arg2);
            stringBuilder.AppendLine();
        }
        public static void AppendLine(this StringBuilder stringBuilder, int lineCount)
        {
            for (int i = 0; i < lineCount; i++)
            {
                stringBuilder.AppendLine();
            }
        }
    }
}
