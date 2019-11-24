using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Globalization
{
    /// <summary>
    /// 提供将数字转换为中文数字的方法，例如将123转换为一百二十三
    /// </summary>
    public class ChineseNumberFormat : ICustomFormatter, IFormatProvider
    {
        #region 单例模式
        private static ChineseNumberFormat instance;

        private ChineseNumberFormat()
        {
        }

        public static ChineseNumberFormat Instance
        {
            get
            {
                if (object.ReferenceEquals(instance, null))
                {
                    instance = new ChineseNumberFormat();
                }
                return instance;
            }
        }
        #endregion
        public object GetFormat(Type formatType)
        { 
            return (formatType == typeof(ICustomFormatter))
                ? this : null;
        }
        #region ICustomFormatter 成员

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            string val = Convert.ToString(arg);
            switch (format)
            {
                case "n"://大写
                case "N"://大写
                    return ChineseNum.GetNormalNumberText(val);
                case "s"://简单读法
                case "S"://简写读法
                    return ChineseNum.GetSampleNumberText(val);
                case "j"://金额数字
                case "J"://金额数字
                    return ChineseNum.GetMoneyText(val);
                default:
                    return ChineseNum.GetNormalNumberText(val);
            }
        }

        #endregion
      
    }




    internal class ChineseNumDataInfo
    {
        static ChineseNumDataInfo()
        {
            Normal = new ChineseNumDataInfo()
            {
                ZeroSegment = "零",
                SmallUnit = new string[] { "", "十", "百", "千" },
                //http://sobar.soso.com/tie/62156493.html?ch=tie.xg.bottom
                BigUnit = new string[] { "", "万", "亿", "兆", "京", "垓", "秭", "穰", "沟", "涧", "正", "载", "极", "恒河沙", "阿僧只", "那由他", "不可思议", "无量", "大数" },
                Dot = "点",
                Nums = new string[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" },
            };
            Money = new ChineseNumDataInfo()
            {
                ZeroSegment = "零",
                SmallUnit = new string[] { "", "拾", "佰", "仟" },
                //http://sobar.soso.com/tie/62156493.html?ch=tie.xg.bottom
                BigUnit = new string[] { "", "万", "亿", "兆", "京", "垓", "秭", "穰", "沟", "涧", "正", "载", "极", "恒河沙", "阿僧只", "那由他", "不可思议", "无量", "大数" },
                Dot = "点",
                Nums = new string[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" },
                DecimalUnit = new string[] { "角", "分", "厘", "毫" }
            };

        }
        private ChineseNumDataInfo()
        {
        }
        public string[] SmallUnit;
        public string[] BigUnit;
        public string[] Nums;
        public string Dot;
        public string[] DecimalUnit;
        public string ZeroSegment;

        public static ChineseNumDataInfo Normal;
        public static ChineseNumDataInfo Money;

    }
    internal class ChineseNum
    {
        public bool IsEmpty { get; set; }

        public string Content { get; set; }

        public static ChineseNum Merger(ChineseNumDataInfo numInfo, ChineseNum[] datas, bool insertZeroAtBeginIfFirstIsEmtpy, string unit)
        {
            Debug.Assert(datas != null && datas.Length > 0);
            bool atLeastHasOneData = false;
            bool lastisempty = false;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < datas.Length; i++)
            {
                if (datas[i].IsEmpty)
                {
                    lastisempty = true;
                }
                else
                {
                    if (lastisempty)
                    {
                        if (atLeastHasOneData || insertZeroAtBeginIfFirstIsEmtpy)
                        {
                            sb.Append(numInfo.ZeroSegment);
                        }
                        sb.Append(datas[i].Content);
                    }
                    else
                    {
                        sb.Append(datas[i].Content);
                    }
                    lastisempty = false;
                    atLeastHasOneData = true;
                }
            }
            if (atLeastHasOneData)
            {
                return new ChineseNum() { IsEmpty = false, Content = sb.ToString() + unit };

            }
            else
            {
                return new ChineseNum() { IsEmpty = true, Content = numInfo.ZeroSegment + unit };

            }
        }
        // 把不超过一万的数转为汉字
        private static ChineseNum GetSmallIntText(ChineseNumDataInfo numdataInfo, string val, string unit)
        {
            System.Diagnostics.Debug.Assert(val.Length < 5);
            ChineseNum[] parts = new ChineseNum[4];
            int q = (val.Length >= 4) ? val[val.Length - 4] - '0' : 0;
            int b = (val.Length >= 3) ? val[val.Length - 3] - '0' : 0;
            int s = (val.Length >= 2) ? val[val.Length - 2] - '0' : 0;
            int g = (val.Length >= 1) ? val[val.Length - 1] - '0' : 0;
            parts[0] = new ChineseNum() { IsEmpty = q == 0, Content = numdataInfo.Nums[q] + numdataInfo.SmallUnit[3], };
            parts[1] = new ChineseNum() { IsEmpty = b == 0, Content = numdataInfo.Nums[b] + numdataInfo.SmallUnit[2], };
            parts[2] = new ChineseNum() { IsEmpty = s == 0, Content = (q == 0 && b == 0 && s == 1) ? numdataInfo.SmallUnit[1] : numdataInfo.Nums[s] + numdataInfo.SmallUnit[1], };//十需要单独处理
            parts[3] = new ChineseNum() { IsEmpty = g == 0, Content = numdataInfo.Nums[g] + numdataInfo.SmallUnit[0], };
            return ChineseNum.Merger(numdataInfo, parts, false, unit);
        }
        //将大整数转为汉字
        private static ChineseNum GetIntText(ChineseNumDataInfo numDataInfo, string val, string unit)
        {
            int len = val.Length;
            int count = (int)Math.Ceiling(len / 4.0);
            ChineseNum[] parts = new ChineseNum[count];
            int firstlen = (len % 4 == 0) ? 4 : len % 4;
            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                {
                    parts[i] = GetSmallIntText(numDataInfo,
                      val.Substring(0, firstlen), numDataInfo.BigUnit[count - i - 1]);
                }
                else
                {
                    parts[i] = GetSmallIntText(numDataInfo,
                        val.Substring(firstlen + (i - 1) * 4, 4), numDataInfo.BigUnit[count - i - 1]);
                }
            }
            var res = ChineseNum.Merger(numDataInfo, parts, false, unit);
            return res;
        }

        private static ChineseNum GetSamplyDecimalText(ChineseNumDataInfo numDataInfo, string val)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < val.Length; i++)
            {
                var ch = val[i] - '0';
                sb.Append(numDataInfo.Nums[ch]);
            }
            return new ChineseNum() { Content = sb.ToString() };
        }

        private static ChineseNum GetDecimal(ChineseNumDataInfo numDataInfo, string text)
        {
            if (numDataInfo.DecimalUnit != null)
            {
                List<ChineseNum> lst = new List<ChineseNum>();
                for (int i = 0; i < text.Length && i < numDataInfo.DecimalUnit.Length; i++)
                {
                    var ch = text[i] - '0';
                    lst.Add(new ChineseNum() { IsEmpty = ch == 0, Content = numDataInfo.Nums[ch] + numDataInfo.DecimalUnit[i] });
                }
                return ChineseNum.Merger(numDataInfo, lst.ToArray(), true, string.Empty);
            }
            else
            {
                return GetSamplyDecimalText(numDataInfo, text);
            }
        }

        public static string GetNormalNumberText(string val)
        {
            if (string.IsNullOrEmpty(val)) throw new ArgumentNullException("val");
            int index = val.IndexOf('.');
            if (index > 0)
            {
                var intpart = val.Substring(0, index);
                if (index == val.Length - 1)
                {
                    return GetIntText(ChineseNumDataInfo.Normal, val, string.Empty).Content;
                }
                else
                {
                    var decimalPart = val.Substring(index + 1);
                    return string.Format("{0}{1}{2}",
                        GetSamplyDecimalText(ChineseNumDataInfo.Normal, intpart).Content,
                        ChineseNumDataInfo.Normal.Dot,
                        GetSamplyDecimalText(ChineseNumDataInfo.Normal, decimalPart).Content
                        );
                }
            }
            else//全部是整数
            {
                return GetIntText(ChineseNumDataInfo.Normal, val, string.Empty).Content;
            }
        }

        public static string GetSampleNumberText(string val)
        {
            if (string.IsNullOrEmpty(val)) throw new ArgumentNullException("val");
            int index = val.IndexOf('.');
            if (index > 0)
            {
                var intpart = val.Substring(0, index);
                if (index == val.Length - 1)
                {
                    return GetSamplyDecimalText(ChineseNumDataInfo.Normal, val).Content;
                }
                else
                {
                    var decimalPart = val.Substring(index + 1);
                    return string.Format("{0}{1}{2}",
                        GetSamplyDecimalText(ChineseNumDataInfo.Normal, intpart).Content,
                        ChineseNumDataInfo.Normal.Dot,
                        GetSamplyDecimalText(ChineseNumDataInfo.Normal, decimalPart).Content
                        );
                }
            }
            else//全部是整数
            {
                return GetSamplyDecimalText(ChineseNumDataInfo.Normal, val).Content;
            }
        }

        public static string GetMoneyText(string val)
        {
            if (string.IsNullOrEmpty(val)) throw new ArgumentNullException("val");
            int index = val.IndexOf('.');
            if (index > 0)
            {
                var intpart = val.Substring(0, index);
                if (index == val.Length - 1)
                {
                    return GetIntText(ChineseNumDataInfo.Money, val, "元整").Content;
                }
                else
                {
                    var decimalPart = val.Substring(index + 1);
                    var decimalPart2 = GetDecimal(ChineseNumDataInfo.Money, decimalPart);
                    if (decimalPart2.IsEmpty)
                    {
                        return GetIntText(ChineseNumDataInfo.Money, intpart, "元整").Content;
                    }
                    else
                    {
                        return string.Format("{0}{1}",
                            GetIntText(ChineseNumDataInfo.Money, intpart, "元").Content,
                            decimalPart2.Content
                            );
                    }
                }
            }
            else//全部是整数
            {
                return GetIntText(ChineseNumDataInfo.Money, val, "元整").Content;
            }
        }
    }
}
