using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace System.Security
{
    public static class HashUtility
    {
        static MD5 md5;
        static HashUtility()
        {

            md5 = MD5.Create();
        }
        /// <summary>
        /// 计算指定字串的MD5哈希字串
        /// </summary>
        /// <param name="str">输入的字符串</param>
        /// <returns>计算出的MD5哈希字串</returns>
        public static string ToMd5HashString(string str)
        {
            return ToMd5HashString(str, MD5HashLength.Lenth32);
        }
        /// <summary>
        /// 计算指定字串的MD5哈希字串
        /// </summary>
        /// <param name="str">输入的字符串</param>
        /// <param name="lengthType">计算结果的长度</param>
        /// <returns></returns>
        public static string ToMd5HashString(string str, MD5HashLength lengthType)
        {
            return ToMd5HashString(str, lengthType, false);
        }
        /// <summary>
        ///  计算指定字串的MD5哈希字串
        /// </summary>
        /// <param name="str">输入的字符串</param>
        /// <param name="lengthType">计算结果的长度</param>
        /// <param name="toLowerCase">是否将结果转为小写,默认为大写</param>
        /// <returns></returns>
        public static string ToMd5HashString(string str, MD5HashLength lengthType, bool toLowerCase)
        {
            string s = str == null ? string.Empty : str;
            return ToMd5HashString(Encoding.UTF8.GetBytes(s), lengthType, toLowerCase);
        }
        /// <summary>
        /// 计算指定byte数组的MD5哈希字串
        /// </summary>
        /// <param name="bytes">输入的字节数组</param>
        /// <returns></returns>
        public static string ToMd5HashString(byte[] bytes)
        {
            return ToMd5HashString(bytes, MD5HashLength.Lenth32);
        }
        /// <summary>
        /// 计算指定byte数组的MD5哈希字串
        /// </summary>
        /// <param name="bytes">输入的字节数组</param>
        /// <param name="lengthType">计算结果的长度</param>
        /// <returns></returns>
        public static string ToMd5HashString(byte[] bytes, MD5HashLength lengthType)
        {
            return ToMd5HashString(bytes, lengthType, false);
        }
        /// <summary>
        /// 计算指定byte数组的MD5哈希字串
        /// </summary>
        /// <param name="bytes">输入的字节数组</param>
        /// <param name="lengthType">计算结果的长度</param>
        /// <param name="toLowerCase">是否将结果转为小写,默认为大写</param>
        /// <returns></returns>
        public static string ToMd5HashString(byte[] bytes, MD5HashLength lengthType, bool toLowerCase)
        {
            byte[] bys = ToMd5HashBytes(bytes);
            StringBuilder sBuilder = new StringBuilder();
            if (toLowerCase) //小写
            {
                for (int i = 0; i < bys.Length; i++)
                {
                    sBuilder.Append(bys[i].ToString("x2"));
                }
            }
            else//大写
            {
                for (int i = 0; i < bys.Length; i++)
                {
                    sBuilder.Append(bys[i].ToString("X2"));
                }
            }
            switch (lengthType)
            {
                case MD5HashLength.Lenth32:
                    return sBuilder.ToString();
                case MD5HashLength.Lenth16:
                    return sBuilder.ToString(8, 16);
                default:
                    return sBuilder.ToString();
            }
        }
        /// <summary>
        /// 计算指定流的MD5哈希字串
        /// </summary>
        /// <param name="stream">输入的流</param>
        /// <param name="lengthType">计算结果的长度</param>
        /// <param name="toLowerCase">是否将结果转为小写,默认为大写</param>
        /// <returns></returns>
        public static string ToMd5HashString(Stream stream, MD5HashLength lengthType, bool toLowerCase)
        {
            byte[] bys = ToMd5HashBytes(stream);
            StringBuilder sBuilder = new StringBuilder();
            if (toLowerCase) //小写
            {
                for (int i = 0; i < bys.Length; i++)
                {
                    sBuilder.Append(bys[i].ToString("x2"));
                }
            }
            else//大写
            {
                for (int i = 0; i < bys.Length; i++)
                {
                    sBuilder.Append(bys[i].ToString("X2"));
                }
            }
            switch (lengthType)
            {
                case MD5HashLength.Lenth32:
                    return sBuilder.ToString();
                case MD5HashLength.Lenth16:
                    return sBuilder.ToString(8, 16);
                default:
                    return sBuilder.ToString();
            }

        }
        /// <summary>
        /// 计算指定流的MD5哈希字串
        /// </summary>
        /// <param name="stream">输入的流</param>
        /// <param name="lengthType">计算结果的长度</param>
        /// <returns></returns>
        public static string ToMd5HashString(Stream stream, MD5HashLength lengthType)
        {
            return ToMd5HashString(stream, MD5HashLength.Lenth32, false);
        }
        /// <summary>
        /// 计算指定流的MD5哈希字串
        /// </summary>
        /// <param name="stream">输入的流</param>
        /// <returns></returns>
        public static string ToMd5HashString(Stream stream)
        {
            return ToMd5HashString(stream, MD5HashLength.Lenth32);
        }
        /// <summary>
        /// 计算指定字符串的MD5哈希值
        /// </summary>
        /// <param name="str">输入的字符串</param>
        /// <returns></returns>
        public static byte[] ToMd5HashBytes(string str)
        {
            string s = str == null ? string.Empty : str;
            return ToMd5HashBytes(Encoding.UTF8.GetBytes(s));
        }
        /// <summary>
        /// 计算指定byte数组的MD5哈希值
        /// </summary>
        /// <param name="bytes">输入的字节数组</param>
        /// <returns></returns>
        public static byte[] ToMd5HashBytes(byte[] bytes)
        {
            return md5.ComputeHash(bytes);
        }
        /// <summary>
        /// 计算指定的流的MD5哈希值
        /// </summary>
        /// <param name="stream">输入的流</param>
        /// <returns></returns>
        public static byte[] ToMd5HashBytes(Stream stream)
        {
            return md5.ComputeHash(stream);
        }
    }

    /// <summary>
    /// 表示MD5哈希字串的长度
    /// </summary>
    public enum MD5HashLength
    {
        /// <summary>
        /// 16位长度
        /// </summary>
        Lenth16,
        /// <summary>
        /// 32位长度
        /// </summary>
        Lenth32
    }
}
