using System;
using System.Collections.Generic;
using System.Text;
using System.Security.AccessControl;
using System.Linq;
namespace System.IO
{
    public static class DirectoryEx
    {
        /// <summary>
        /// Creates the directory if not exist.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void CreateDirectoryIfNotExist(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }



        public static IEnumerable<string> GetFiles(string path, string includePatternEx, string excludePatternEx)
        {

            includePatternEx = includePatternEx ?? "*";
            excludePatternEx = excludePatternEx ?? string.Empty;
            var excludes = excludePatternEx.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            var includes = includePatternEx.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            includes = (from p in includes select p.Trim()).ToArray();
            excludes = (from p in excludes select p.Trim()).ToArray();
            return GetFiles(path, includes, excludes);
        }

        public static IEnumerable<string> GetFiles(string path, IEnumerable<string> includesPatterns, IEnumerable<string> excludePatterns)
        {
            foreach (var fullName in System.IO.Directory.GetFiles(path))
            {
                var filename = System.IO.Path.GetFileName(fullName);

                if (filename.IsMatchWildcardAnyOne(includesPatterns, StringComparison.CurrentCultureIgnoreCase) &&
                    !filename.IsMatchWildcardAnyOne(excludePatterns, StringComparison.CurrentCultureIgnoreCase))
                {
                    yield return fullName;
                }
            }
        }
        /// <summary>
        /// 获取符合条件的所有文件,包含子目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="includePatternEx"></param>
        /// <param name="excludePatternEx"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllFiles(string path, string includePatternEx, string excludePatternEx)
        {
            foreach (var v in GetFiles(path, includePatternEx, excludePatternEx))
            {
                yield return v;
            }
            foreach (var v in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            {
                foreach (var file in GetFiles(v, includePatternEx, excludePatternEx))
                {
                    yield return file;
                }
            }

        }
        /// <summary>
        ///  获取符合条件的所有文件,包含子目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="includesPatterns"></param>
        /// <param name="excludePatterns"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAllFiles(string path, IEnumerable<string> includesPatterns, IEnumerable<string> excludePatterns)
        {
            foreach (var v in GetFiles(path, includesPatterns, excludePatterns))
            {
                yield return v;
            }
            foreach (var v in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            {
                foreach (var file in GetFiles(v, includesPatterns, excludePatterns))
                {
                    yield return file;
                }
            }

        }
    }
}
