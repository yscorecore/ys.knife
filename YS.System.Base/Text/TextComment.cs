using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Text
{
    [Serializable]
    public class TextComment
    {
        public TextComment()
        {
            this.InsertPattern = @"^";
        }

        public string InsertPattern { get; set; }
        public RegexOptions InsertOptions { get; set; }
        [Obsolete("放弃使用")]
        public string FindPattern { get; set; }
        [Obsolete("放弃使用")]
        public RegexOptions FindOptions { get; set; }

        /// <summary>
        /// 获取或设置要替换的次数，如果不限制次数，则为0
        /// </summary>
        public int ReplaceCount { get; set; }

        public string InsertReplacement { get; set; }//高级格式

        public virtual InsertCommentResult InsertComment(string text)
        {
            if (text != null)
            {

                string pattern = this.InsertPattern ?? string.Empty;
                RegexOptions ro = this.InsertOptions;
                string replacement = this.InsertPattern ?? string.Empty;
                if (Regex.IsMatch(text, pattern, ro))
                {
                    string res = string.Empty;
                    Regex rg = new Regex(pattern, ro);
                    if (this.ReplaceCount > 0)
                    {
                        res = rg.Replace(text, this.OnGetReplacementText(), this.ReplaceCount);
                    }
                    else
                    {
                        res = rg.Replace(text, this.OnGetReplacementText());
                    }
                    return new InsertCommentResult() { Succeed = true, ResultText = res };
                }
                else
                {
                    return new InsertCommentResult() { Succeed = false, ResultText = string.Empty };
                }

            }
            else
            {
                return new InsertCommentResult() { Succeed = false, ResultText = string.Empty };
            }
        }

        public virtual FindCommentResult FindComment(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new FindCommentResult() { Comment = string.Empty };
            }
            else
            {
                var match = Regex.Match(text, FindPattern ?? string.Empty, FindOptions);
                if (match != null && match.Success)
                {
                    return new FindCommentResult() { Comment = match.Value, CommentStartIndex = match.Index, CommentLength = match.Length };
                }
                else
                {
                    return new FindCommentResult() { Comment = string.Empty, };
                }
            }
        }

        protected virtual string OnGetReplacementText()
        {
            return InsertReplacement ?? string.Empty;
        }
    }

    public class InsertCommentResult
    {
        public bool Succeed { get; set; }
        public string ResultText { get; set; }
    }

    public class FindCommentResult
    {
        public bool HasComment { get { return !string.IsNullOrEmpty(Comment); } }
        public int CommentStartIndex { get; set; }
        public int CommentLength { get; set; }
        public string Comment { get; set; }
    }
}
