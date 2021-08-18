using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace YS.Knife
{
    class CsharpCodeBuilder
    {
        private const int TabSize = 4;
        private StringBuilder stringBuilder = new StringBuilder();

        private int depthOfNesting = 0;
        private void IncreaseDepth()
        {
            this.depthOfNesting++;
        }
        private void DecreaseDepth()
        {
            this.depthOfNesting = Math.Max(0, this.depthOfNesting - 1);
        }
        public void AppendCodeLines(string lines)
        {
            if (string.IsNullOrEmpty(lines)) return;
            using (StringReader sr = new StringReader(lines))
            {
                string whiteSpaces = new string(' ', depthOfNesting * TabSize);
                do
                {
                    string line = sr.ReadLine();
                    if (line == null) break;
                    if (string.IsNullOrEmpty(line))
                    {
                        stringBuilder.AppendLine();
                    }
                    else
                    {
                        stringBuilder.Append(whiteSpaces);
                        stringBuilder.AppendLine(line);
                    }

                }
                while (true);
            }
        }
        public void BeginSegment()
        {
            this.AppendCodeLines("{");
            this.IncreaseDepth();
        }

        public void EndSegment(string end = "}")
        {
            this.DecreaseDepth();
            if (!string.IsNullOrEmpty(end))
            {
                this.AppendCodeLines(end);
            }

        }
        public void EndAllSegments()
        {
            while (depthOfNesting > 0)
            {
                this.DecreaseDepth();
                this.AppendCodeLines("}");
            }

        }

        public void AppendLine()
        {
            stringBuilder.AppendLine();
        }
        public override string ToString()
        {
            return stringBuilder.ToString();
        }
    }
}
