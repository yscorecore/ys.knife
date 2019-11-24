using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("YS.System.Drawing.Core,PublicKey=0024000004800000940000000602000000240000525341310004000001000100f9edcb66367eec63d2b4cbe8a9a89a097883c1c8f87b6af1dcae97af66ccdbc1affcc97be536513df366ce9033f91365f573408e5ca8ca50160fa2099b6b3325f18eb79f6c7dcb0eea78ae80d72fce800643e0dfb1553fd969351df70ad8debf4658893f415a259d1223c7649146901f8166b808275a7191c2a823343d4840a6")]


namespace System
{

    public static class ResourceProvider
    {
        internal const string localPrefiex = @"local:\\";
        internal const string localPrefiex2 = @"local://";
        internal const string resPrefiex = @"res:\\";
        internal const string resPrefiex2 = @"res://";
        internal const string mresPrefiex = @"mres:\\";
        internal const string mresPrefiex2 = @"mres://";

        private static readonly char[] SPCHS = new char[] { '/', '\\' };

        public static string GetResourceText(this Assembly assembly, string resourceUrl)
        {
            if (resourceUrl.StartsWith(localPrefiex, StringComparison.InvariantCultureIgnoreCase)
              || resourceUrl.StartsWith(localPrefiex2, StringComparison.InvariantCultureIgnoreCase))
            {
                using (var stream = GetStreamFromLocalFile( resourceUrl.Substring(localPrefiex.Length)))
                {
                    return stream.ReadAllText();
                }
            }
            else if (resourceUrl.StartsWith(resPrefiex, StringComparison.InvariantCultureIgnoreCase)
                || resourceUrl.StartsWith(resPrefiex2, StringComparison.InvariantCultureIgnoreCase))//嵌入的资源
            {
                using (var stream = GetStreamFromResource(resourceUrl.Substring(resPrefiex.Length)))
                {
                    return stream.ReadAllText();
                }
            }
            else if (resourceUrl.StartsWith(mresPrefiex, StringComparison.InvariantCultureIgnoreCase)
                || resourceUrl.StartsWith(mresPrefiex2, StringComparison.InvariantCultureIgnoreCase))//嵌入的托管资源
            {
                return GetStringFromManagerResource( resourceUrl.Substring(mresPrefiex.Length));
            }
            else
            {
                using (var stream = GetStreamFromWebRequest( resourceUrl))
                {
                    return stream.ReadAllText();
                }
            }

        }

        public static string GetResourceText(this Assembly assembly, string resourceUrl, Encoding encoding)
        {
            encoding = encoding ?? Encoding.UTF8;
            if (resourceUrl.StartsWith(localPrefiex, StringComparison.InvariantCultureIgnoreCase)
              || resourceUrl.StartsWith(localPrefiex2, StringComparison.InvariantCultureIgnoreCase))
            {
                using (var stream = GetStreamFromLocalFile( resourceUrl.Substring(localPrefiex.Length)))
                {
                    return stream.ReadAllText(encoding);
                }
            }
            else if (resourceUrl.StartsWith(resPrefiex, StringComparison.InvariantCultureIgnoreCase)
                || resourceUrl.StartsWith(resPrefiex2, StringComparison.InvariantCultureIgnoreCase))//嵌入的资源
            {
                using (var stream = GetStreamFromResource(resourceUrl.Substring(resPrefiex.Length)))
                {
                    return stream.ReadAllText(encoding);
                }
            }
            else if (resourceUrl.StartsWith(mresPrefiex, StringComparison.InvariantCultureIgnoreCase)
                || resourceUrl.StartsWith(mresPrefiex2, StringComparison.InvariantCultureIgnoreCase))//嵌入的托管资源
            {
                return GetStringFromManagerResource( resourceUrl.Substring(mresPrefiex.Length));
            }
            else if (System.IO.PathEx.IsLocalPath(resourceUrl))
            {
                using (var stream = GetStreamFromLocalFile( resourceUrl))
                {
                    return stream.ReadAllText(encoding);
                }
            }
            else
            {
                using (var stream = GetStreamFromWebRequest( resourceUrl))
                {
                    return stream.ReadAllText(encoding);
                }
            }

        }

        public static byte[] GetResourceBytes(this Assembly assembly, string resourceUrl)
        {
            if (resourceUrl.StartsWith(localPrefiex, StringComparison.InvariantCultureIgnoreCase)
                || resourceUrl.StartsWith(localPrefiex2, StringComparison.InvariantCultureIgnoreCase))
            {
                using (var stream = GetStreamFromLocalFile( resourceUrl.Substring(localPrefiex.Length)))
                {
                    return stream.ToArray();
                }
            }
            else if (resourceUrl.StartsWith(resPrefiex, StringComparison.InvariantCultureIgnoreCase)
                || resourceUrl.StartsWith(resPrefiex2, StringComparison.InvariantCultureIgnoreCase))//嵌入的资源
            {
                using (var stream = GetStreamFromResource(resourceUrl.Substring(resPrefiex.Length)))
                {
                    return stream.ToArray();
                }
            }
            else if (resourceUrl.StartsWith(mresPrefiex, StringComparison.InvariantCultureIgnoreCase)
                || resourceUrl.StartsWith(mresPrefiex2, StringComparison.InvariantCultureIgnoreCase))//嵌入的托管资源
            {
                var obj = GetObjectFromManagerResource( resourceUrl.Substring(mresPrefiex.Length));
                if (obj is byte[])
                {
                    return obj as byte[];
                }
                else if (obj is Stream)
                {
                    return (obj as Stream).ToArray();
                }
                else
                {
                    throw new Exception("");
                }
            }
            else if (System.IO.PathEx.IsLocalPath(resourceUrl))
            {
                using (var stream = GetStreamFromLocalFile( resourceUrl))
                {
                    return stream.ToArray();
                }
            }
            else
            {
                using (var stream = GetStreamFromWebRequest( resourceUrl))
                {
                    return stream.ToArray();
                }
            }
        }

        public static Stream GetResourceStream( string resourceUrl)
        {

            if (resourceUrl.StartsWith(localPrefiex, StringComparison.InvariantCultureIgnoreCase)
               || resourceUrl.StartsWith(localPrefiex2, StringComparison.InvariantCultureIgnoreCase))
            {
                return GetStreamFromLocalFile(resourceUrl.Substring(localPrefiex.Length));
            }
            else if (resourceUrl.StartsWith(resPrefiex, StringComparison.InvariantCultureIgnoreCase)
                || resourceUrl.StartsWith(resPrefiex2, StringComparison.InvariantCultureIgnoreCase))//嵌入的资源
            {
                return GetStreamFromResource( resourceUrl.Substring(resPrefiex.Length));
            }
            else if (resourceUrl.StartsWith(mresPrefiex, StringComparison.InvariantCultureIgnoreCase)
                || resourceUrl.StartsWith(mresPrefiex2, StringComparison.InvariantCultureIgnoreCase))//嵌入的托管资源
            {
                var obj = GetObjectFromManagerResource( resourceUrl.Substring(mresPrefiex.Length));
                if (obj is Stream)
                {
                    return obj as Stream;
                }
                else if (obj is byte[])
                {
                    return new MemoryStream(obj as byte[]);
                }
                else
                {
                    throw new Exception("无法获取资源");
                }
            }
            else if (System.IO.PathEx.IsLocalPath(resourceUrl))
            {
               return GetStreamFromLocalFile( resourceUrl);
               
            }
            else
            {
                return GetStreamFromWebRequest( resourceUrl);
            }
        }

        public static T GetResourceObject<T>(string resourceUrl,Func<Stream,T> objectFactory)
        {
            if (objectFactory == null) throw new ArgumentNullException("objectFactory");
            if (resourceUrl.StartsWith(mresPrefiex, StringComparison.InvariantCultureIgnoreCase)
               || resourceUrl.StartsWith(mresPrefiex2, StringComparison.InvariantCultureIgnoreCase))//嵌入的托管资源
            {
                return (T)GetObjectFromManagerResource(resourceUrl.Substring(mresPrefiex.Length));
            }
            else
            {
                using (var stream =GetResourceStream( resourceUrl))
                {
                    return objectFactory(stream);
                }
            }
        }

        internal static Stream GetStreamFromLocalFile(string resourceUrl)
        {
            var fullpath = System.IO.PathEx.GetFullPath(resourceUrl);
            return File.OpenRead(fullpath);
        }

        internal static Stream GetStreamFromResource(string resourceUrl)
        {
            var items = resourceUrl.Split(SPCHS, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length != 2)
            {
                throw new Exception("格式错误");
            }
            else
            {
                var assembly = GetAssembly(items[0]);
                return assembly.GetManifestResourceStream(items[1]);
            }
        }

        internal static Stream GetStreamFromWebRequest(string resourceUrl)
        {
            var request = WebRequest.Create(resourceUrl);
            return request.GetResponse().GetResponseStream();
        }

        internal static object GetObjectFromManagerResource(string resourceUrl)
        {
            var items = resourceUrl.Split(SPCHS, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length != 3)
            {
                throw new ApplicationException(string.Format("资源Url‘{0}’格式错误。", resourceUrl));
            }
            else
            {
               var assembly = GetAssembly(items[0]);
                var rman = ResourceManagerFactory.GetResourceManager(assembly, items[1]);
                return rman.GetObject(items[2]);
            }
        }

        internal static string GetStringFromManagerResource(string resourceUrl)
        {
            var items = resourceUrl.Split(SPCHS, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length != 3)
            {
                throw new ApplicationException(string.Format("资源Url‘{0}’格式错误。", resourceUrl));
            }
            else
            {
                var assembly = GetAssembly(items[0]);
                var rman = ResourceManagerFactory.GetResourceManager(assembly, items[1]);
                return rman.GetString(items[2]);
            }

        }

        internal static Assembly GetAssembly(string assemblyName)
        {
            return Assembly.Load(assemblyName);
        }

    }
}
