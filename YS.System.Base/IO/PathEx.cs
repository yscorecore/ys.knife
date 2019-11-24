using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.IO
{
    public static class PathEx
    {
        /// <summary>
        /// 获取指定路径对于另外一个路径的相对路径
        /// </summary>
        /// <param name="fullDestinationPath">路径的全名</param>
        /// <param name="startPath">需要起始的路径</param>
        /// <returns>返回相对路径</returns>
        public static string GetRelativePath(string fullDestinationPath, string startPath)
        {
            string[] l_startPathParts = Path.GetFullPath(startPath).Trim(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar);
            string[] l_destinationPathParts = fullDestinationPath.Split(Path.DirectorySeparatorChar);

            int l_sameCounter = 0;
            while ((l_sameCounter < l_startPathParts.Length) && (l_sameCounter < l_destinationPathParts.Length) && l_startPathParts[l_sameCounter].Equals(l_destinationPathParts[l_sameCounter], StringComparison.InvariantCultureIgnoreCase))
            {
                l_sameCounter++;
            }

            if (l_sameCounter == 0)
            {
                return fullDestinationPath; // There is no relative link.
            }

            StringBuilder l_builder = new StringBuilder();
            for (int i = l_sameCounter; i < l_startPathParts.Length; i++)
            {
                l_builder.Append(".." + Path.DirectorySeparatorChar);
            }

            for (int i = l_sameCounter; i < l_destinationPathParts.Length; i++)
            {
                l_builder.Append(l_destinationPathParts[i] + Path.DirectorySeparatorChar);
            }
            if (l_builder.Length > 0)
            {
                l_builder.Length--;
            }
            return l_builder.ToString();
        }

        public static string GetLocalPathUnescaped(string url)
        {
            string text1 = "file:///";
            if (url.StartsWith(text1, StringComparison.OrdinalIgnoreCase))
            {
                return url.Substring(text1.Length);
            }
            return GetLocalPath(url);
        }
        public static string GetFullPath(string path)
        {
            if (System.IO.Path.IsPathRooted(path))
            {
                return path;
            }
            else
            {
                //if (EnvironmentEx.AppKind == AppKind.WinApp)
                //{
                //    return System.IO.Path.GetFullPath(path);
                //}
                //else
                {
                    var res= System.IO.Path.Combine(EnvironmentEx.ApplicationDirectory, path);

                    return System.IO.Path.GetFullPath(res);//处理相对路径的情况 如 c:\windows\system32\..\notepad.exe
                }
            }
        }
        /// <summary>
        /// 判断一个路径是否是本地文件
        /// 如：c:\windows\notepad.exe ;c:\windows 则返回true
        /// </summary>
        /// <param name="urlOrPath"></param>
        /// <returns></returns>
        public static bool IsLocalPath(string urlOrPath)
        {
            Uri uri = null;
            if (Uri.TryCreate(urlOrPath, UriKind.Absolute, out uri))
            {
                return uri.Scheme == Uri.UriSchemeFile;
            }
            if (Uri.TryCreate(urlOrPath, UriKind.Relative, out uri))
            {
                return true;
            }
            return false;

        }
        private static string GetLocalPath(string fileName)
        {
            Uri uri1 = new Uri(fileName);
            return (uri1.LocalPath + uri1.Fragment);
        }
        /// <summary>
        /// 给指定的文件名称添加后缀名
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="extensionName"></param>
        /// <returns></returns>
        public static string ComboExtension(string fileName, string extensionName)
        {
            extensionName = extensionName ?? string.Empty;
            extensionName = extensionName.Trim();
            if (extensionName.Length != 0)
            {
                if (extensionName[0] == '.')
                {
                    return fileName + extensionName;
                }
                else
                {
                    return fileName + '.' + extensionName;
                }
            }
            else
            {
                return fileName;
            }
        }

        /// <summary>
        /// 判断路径是否是绝对路径或者是合法的Uri
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsAbsolutePathOrUri(string path)
        {
            Uri uri = null;
            return Uri.TryCreate(path, UriKind.Absolute, out uri);
        }
        /// <summary>
        /// 判断字符串是否是合法的文件名（不包括文件夹）
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsValidFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;
            foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
            {
                if (fileName.Contains(rInvalidChar))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 判断字符串是否是合法的路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsValidPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            foreach (char rInvalidChar in Path.GetInvalidPathChars())
            {
                if (path.Contains(rInvalidChar))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 获取一个新文件名
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        /// <example>
        /// GetNewFileName("picture.jpg", "{time}_{yyyy}_{mm}_{dd}_{rand}_{guid}_{file}")
        /// </example>
        public static string GetNewFileName(string format)
        {
            if (string.IsNullOrEmpty(format)) format = "{guid}";
            return Regex.Replace(format, @"{(?<fmt>\w+)(:(?<arg>\w))?}", (a) =>
            {
                var fmt = a.Groups["fmt"].Value;
                switch (fmt)
                {
                    case "time":
                        return DateTime.Now.Ticks.ToString();
                    case "rand":
                        return RandomUtility.RandomCode("0123456789", a.Groups["arg"].Value.AsInt32(6));
                    case "guid":
                        string arg = a.Groups["arg"].Value;
                        switch (arg.ToUpper())
                        {
                            case "N":
                            case "D":
                            case "B":
                            case "P":
                            case "X":
                                return Guid.NewGuid().ToString(arg);
                            default:
                                return Guid.NewGuid().ToString();
                        }
                    case "yy":
                        return (DateTime.Now.Year % 100).ToString("d2");
                    case "yyyy":
                        return DateTime.Now.Year.ToString();
                    case "mm":
                        return DateTime.Now.Month.ToString("d2");
                    case "dd":
                        return DateTime.Now.Day.ToString("d2");
                    case "hh":
                        return DateTime.Now.Hour.ToString("d2");
                    case "ii":
                        return DateTime.Now.Minute.ToString("d2");
                    case "ss":
                        return DateTime.Now.Second.ToString("d2");
                    default:
                        return a.Value;
                }
            });

        }

        public static string GetNewFileName(string format, string filename,bool comboExtension=true)
        {
            string filenamewithnoext = System.IO.Path.GetFileNameWithoutExtension(filename)??string.Empty;
            string extname = System.IO.Path.GetExtension(filename);
            if (string.IsNullOrEmpty(format)) format = "{guid}";
            var res = Regex.Replace(format, @"{(?<fmt>\w+)(:(?<arg>\w))?}", (a) =>
            {
                var fmt = a.Groups["fmt"].Value;
                switch (fmt)
                {
                    case "time":
                        return DateTime.Now.Ticks.ToString();
                    case "rand":
                        return RandomUtility.RandomCode("0123456789", a.Groups["arg"].Value.AsInt32(6));
                    case "guid":
                        string arg = a.Groups["arg"].Value;
                        switch (arg.ToUpper())
                        {
                            case "N":
                            case "D":
                            case "B":
                            case "P":
                            case "X":
                                return Guid.NewGuid().ToString(arg);
                            default:
                                return Guid.NewGuid().ToString();
                        }
                    case "yy":
                        return (DateTime.Now.Year % 100).ToString("d2");
                    case "yyyy":
                        return DateTime.Now.Year.ToString();
                    case "mm":
                        return DateTime.Now.Month.ToString("d2");
                    case "dd":
                        return DateTime.Now.Day.ToString("d2");
                    case "hh":
                        return DateTime.Now.Hour.ToString("d2");
                    case "ii":
                        return DateTime.Now.Minute.ToString("d2");
                    case "ss":
                        return DateTime.Now.Second.ToString("d2");
                    case "filename":
                    case "file":
                        return filenamewithnoext;
                    default:
                        return a.Value;
                }
            });
            if (comboExtension)
            {
                return ComboExtension(res, extname);
            }
            else
            {
                return res;
            }
           

        }

        /// <summary>
        /// 将路径作为目录名称，即后缀作为目录的分隔符
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string AsDirectoryName(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString())) path += Path.DirectorySeparatorChar;
            return path;
        }
    }
}
