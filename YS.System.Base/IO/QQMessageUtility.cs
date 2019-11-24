using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
namespace System.Attendance
{

    public class QQTextMessageInfo
    {
        public DateTime DateTime { get; set; }
        public string User { get; set; }
        public string Content { get; set; }
    }
    public class QQMessageReader :IDisposable
    {
        static Regex reg = new Regex(@"(?<year>\d{4})-(?<month>\d{1,2})-(?<day>\d{1,2}) (?<hour>\d{1,2}):(?<min>\d{1,2}):(?<sec>\d{1,2}) (?<m>AM|PM)\s+(?<name>.+)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        string[] items=new string[0];

        public QQMessageReader(string textFilePath, Encoding encoding)
        {
            string text = File.ReadAllText(textFilePath, encoding);
            items = text.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
        
        public QQMessageReader(string textFilePath)
            : this(textFilePath, Encoding.UTF8)
        {
        }
     

        public IEnumerable<QQTextMessageInfo> GetMessages()
        {
            if (items != null)
            {
                foreach (var v in this.items)
                {
                    string[] subitmes = v.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (subitmes.Length >=1)
                    {
                        Match match = reg.Match(subitmes[0]);
                        if (match.Success)
                        {
                            var res = this.GetMessage(match);
                            StringBuilder sb = new StringBuilder();
                            for (int i = 1; i < subitmes.Length; i++)
                            {
                                sb.AppendLine(subitmes[i]);
                            }
                            res.Content = sb.ToString();
                            yield return res;
                        }
                    }
                }
            }
        }
        private QQTextMessageInfo GetMessage(Match res)
        {
            int year = int.Parse(res.Groups["year"].Value);
            int month = int.Parse(res.Groups["month"].Value);
            int day = int.Parse(res.Groups["day"].Value);
            int h = int.Parse(res.Groups["hour"].Value);
            int m = int.Parse(res.Groups["min"].Value);
            int s = int.Parse(res.Groups["sec"].Value);
            bool ispm = string.Equals(res.Groups["m"].Value, "PM", StringComparison.InvariantCultureIgnoreCase);
            string name = res.Groups["name"].Value;
            DateTime time = new DateTime(year, month, day, h , m, s, DateTimeKind.Local);
            if (time.Hour < 12 && ispm) { time = time.AddHours(12); };
            QQTextMessageInfo message = new QQTextMessageInfo()
            {
                DateTime = time,
                User = name,
            };
            return message;
        }


        #region IDisposable 成员

        public void Dispose()
        {
            items = null;
        }

        #endregion
    }

    

}
