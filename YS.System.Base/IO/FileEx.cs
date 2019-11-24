using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace System.IO
{
    public static class FileEx
    {

        public static string ReadAllText(string fileName)
        {
            var encoding = GetFileEncoding(fileName);
            return File.ReadAllText(fileName, encoding);
        }
        public static Encoding GetFileEncoding(string fileName)
        {
            return Text.EncodingEx.GetFileEncoding(fileName);
        }
        /// <summary>
        /// 替换指定文件中的字符串
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        public static void ReplaceText(string fileName, string oldValue, string newValue)
        {
            if (!string.IsNullOrEmpty(oldValue))
            {
                newValue = newValue ?? string.Empty;
                var encoding = Text.EncodingEx.GetFileEncoding(fileName);
                string text = File.ReadAllText(fileName, encoding);
                File.WriteAllText(fileName, text.Replace(oldValue, newValue), encoding);
            }
        }
        /// <summary>
        /// Replaces the text.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        public static void ReplaceText(string fileName, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (!string.IsNullOrEmpty(oldValue))
            {
                newValue = newValue ?? string.Empty;
                var encoding = Text.EncodingEx.GetFileEncoding(fileName);
                string text = File.ReadAllText(fileName, encoding);
                File.WriteAllText(fileName, text.Replace(oldValue, newValue, comparisonType), encoding);
            }
        }
        /// <summary>
        /// Replaces the text.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="replaces">The replaces.</param>
        public static void ReplaceText(string fileName, Dictionary<string, string> replaces)
        {
            if (replaces != null && replaces.Count > 0)
            {
                var encoding = Text.EncodingEx.GetFileEncoding(fileName);
                string text = File.ReadAllText(fileName, encoding);
                File.WriteAllText(fileName, text.Replace(replaces), encoding);
            }
        }
        /// <summary>
        /// Replaces the text.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="replaces">The replaces.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        public static void ReplaceText(string fileName, Dictionary<string, string> replaces, StringComparison comparisonType)
        {
            if (replaces != null && replaces.Count > 0)
            {
                var encoding = Text.EncodingEx.GetFileEncoding(fileName);
                string text = File.ReadAllText(fileName, encoding);
                File.WriteAllText(fileName, text.Replace(replaces, comparisonType), encoding);
            }
        }

        public static void ReplaceTextWithRegex(string fileName, string pattern, string replacement)
        {
            var encoding = Text.EncodingEx.GetFileEncoding(fileName);
            string text = File.ReadAllText(fileName, encoding);
            text = Regex.Replace(text, pattern, replacement);
            File.WriteAllText(fileName,text, encoding);
        }
        public static void ReplaceTextWithRegex(string fileName, string pattern, string replacement,RegexOptions options)
        {
            var encoding = Text.EncodingEx.GetFileEncoding(fileName);
            string text = File.ReadAllText(fileName, encoding);
            text = Regex.Replace(text, pattern, replacement, options);
            File.WriteAllText(fileName, text, encoding);
        }
        public static void ReplaceTextWithRegex(string fileName, string pattern, MatchEvaluator matchEvaluator)
        {
            var encoding = Text.EncodingEx.GetFileEncoding(fileName);
            string text = File.ReadAllText(fileName, encoding);
            Regex.Replace(text, pattern, matchEvaluator);
            File.WriteAllText(fileName, text, encoding);
        }
        public static void ReplaceTextWithRegex(string fileName, string pattern, MatchEvaluator matchEvaluator, RegexOptions options)
        {
            var encoding = Text.EncodingEx.GetFileEncoding(fileName);
            string text = File.ReadAllText(fileName, encoding);
            Regex.Replace(text, pattern, matchEvaluator,options);
            File.WriteAllText(fileName, text, encoding);
        }
        /// <summary>
        /// 获取文件的hash值
        /// </summary>
        /// <param name="file">输入的文件路径</param>
        /// <param name="hashName">哈希名称，默认为Md5</param>
        /// <returns></returns>
        public static string GetFileHash(string file, string hashName = "MD5")
        {
            using (var stream = System.IO.File.OpenRead(file))
            {
                using (var hash = HashAlgorithm.Create(hashName))
                {
                    var bys = hash.ComputeHash(stream);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < bys.Length; i++)
                    {
                        sb.Append(bys[i].ToString("X2"));
                    }
                    return sb.ToString();
                }
            }
        }
    }
}
