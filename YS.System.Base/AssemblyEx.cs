using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace System
{
    public static class AssemblyEx
    {
        static AssemblyEx()
        {
            tempFolderName = Assembly.GetEntryAssembly().GetName().Name;
        }

        private readonly static string tempFolderName;

        public static string TempFolderName
        {
            get
            {
                return
                    System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    AssemblyEx.tempFolderName);
            }
        }

        private static Dictionary<string, string> dic = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// 释放嵌入的程序集并且监视程序集加载失败时请求自动加载程序集
        /// </summary>
        /// <param name="root"></param>
        public static void ReleaseAndWatchAssemblies(string root)
        {
            Assembly curAsm = Assembly.GetCallingAssembly();
            if (string.IsNullOrEmpty(root)) root = curAsm.GetName().Name;
            root.TrimEnd(new char[] { ' ', '.' });
            System.IO.Directory.CreateDirectory(TempFolderName);
            foreach (var resource in curAsm.GetManifestResourceNames())
            {
                if (resource.StartsWith(root, StringComparison.InvariantCultureIgnoreCase))
                {
                    string assemblyname = resource.Substring(root.Length + 1);
                    string fileName = Path.Combine(TempFolderName, assemblyname);
                    string extname = System.IO.Path.GetExtension(fileName);
                    extname = extname.ToUpper();
                    bool isdllorexe = (extname == ".EXE" || extname == ".DLL");
                    using (Stream stream = curAsm.GetManifestResourceStream(resource))
                    {
                        byte[] bys = new byte[stream.Length];
                        stream.Read(bys, 0, bys.Length);
                        if (File.Exists(fileName))
                        {//存在同名的文件,则替换文件
                            if (GetHash(fileName) != GetHash(bys))
                            {
                                File.WriteAllBytes(fileName, bys);//可能会写入失败
                            }
                        }
                        else
                        {
                            File.WriteAllBytes(fileName, bys);
                        }
                        if (isdllorexe)
                        {
                            string key = System.IO.Path.GetFileNameWithoutExtension(assemblyname);
                            dic[key] = fileName;
                        }
                    }
                }
            }
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName an = new AssemblyName(args.Name);
            if (an != null && dic.ContainsKey(an.Name))
            {
                string fullname = dic[an.Name];
                return Assembly.LoadFrom(fullname);
            }
            return null;
        }

        static string GetHash(string file)
        {
            return GetHash(File.ReadAllBytes(file));
        }
        static string GetHash(byte[] bys)
        {
            using (var hash = HashAlgorithm.Create())
            {
                return BitConverter.ToString(hash.ComputeHash(bys));
            }
        }
    }
}
