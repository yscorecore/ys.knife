using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace System.Text
{
    public static class EncodingEx
    {
        static Encoding gb2312 = null;
        static Encoding gbk = null;
        public static Encoding GB2312
        {
            get 
            {
                if (gb2312 == null) gb2312 = Encoding.GetEncoding("gb2312");
                return gb2312;
            }
        }
        public static Encoding GBK
        {
            get
            {
                if (gbk == null) gbk = Encoding.GetEncoding("GBK");
                return gbk;
            }
        }
        /// <summary>
        /// 获取指定文件的编码方式
        /// </summary>
        /// <param name="srcFile">文件的路径名称</param>
        /// <returns>该文件的编码方式</returns>
        public static Encoding GetFileEncoding(string srcFile)
        {
            if (!File.Exists(srcFile)) throw new FileNotFoundException();
            // *** Detect byte order mark if any - otherwise assume default
            byte[] buffer = new byte[5];
            using (FileStream file = new FileStream(srcFile, FileMode.Open))
            {
                file.Read(buffer, 0, 5);
            }
            return ByteToEncoding( buffer);
        }

        private static Encoding ByteToEncoding(byte[] buffer)
        {
            Encoding enc = Encoding.Default;
            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                enc = Encoding.UTF8;
            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                enc = Encoding.Unicode;
            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                enc = Encoding.UTF32;
            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                enc = Encoding.UTF7;
            else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                // 1201 unicodeFFFE Unicode (Big-Endian)
                enc = Encoding.GetEncoding(1201);
            else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                // 1200 utf-16 Unicode
                enc = Encoding.GetEncoding(1200);
            return enc;
        }

        /// <summary>
        /// 获取指定文件的编码方式
        /// </summary>
        /// <param name="srcFile">文件的路径名称</param>
        /// <returns>该文件的编码方式</returns>
        public static Encoding GetFileEncoding(this FileInfo fileInfo)
        {
            if (fileInfo != null) throw new ArgumentNullException("fileInfo");

            return GetFileEncoding(fileInfo.FullName);
        }
    }
}
