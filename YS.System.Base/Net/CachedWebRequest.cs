using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;

namespace System.Net
{
    public enum CacheLevel
    {
        NoCache,
        CacheFirst,
        RemoteFirst,

    }

    public class CachedWebRequest
    {

        System.Caching.ICacheManager cman = null;
        public Stream GetStream(string url, CacheLevel cacheLevel, string baseDir, string localCacheDir)
        {
            Uri uri = new Uri(url);
            if (string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.InvariantCultureIgnoreCase))
            {
                if (cacheLevel == CacheLevel.NoCache)
                {
                    return LoadFromRequest(uri);
                }
                else
                {
                    var cacheInfo = GetCacheInfo(url);
                    if (cacheInfo == null)//第一次
                    {
                        cacheInfo = new CacheInfo() { Url = uri.ToString() };
                        cman.Set(url, cacheInfo, TimeSpan.FromDays(1), null);
                    }
                    string fileName = this.LoadUrl(cacheInfo, uri, cacheLevel, baseDir, localCacheDir);
                    return File.OpenRead(fileName);
                }
            }
            else
            {
                return LoadFromRequest(uri);
            }
        }

        private string LoadUrl(CacheInfo cacheInfo, Uri url, CacheLevel level, string baseDir, string cacheDir)
        {
            var cacheFileFullPath = string.Empty;
            if (string.IsNullOrEmpty(cacheInfo.FileName))
            {
                cacheFileFullPath = System.IO.Path.Combine(baseDir, cacheDir,
                    string.Format("{0}{1}", cacheInfo.Url.ToMd5Hash(MD5HashLength.Lenth16), System.IO.Path.GetExtension(url.LocalPath)));
                cacheInfo.FileName = System.IO.PathEx.GetRelativePath(cacheFileFullPath, baseDir);
            }
            else
            {
                cacheFileFullPath = System.IO.Path.Combine(baseDir, cacheInfo.FileName);
            }
            if (File.Exists(cacheFileFullPath) && level == CacheLevel.CacheFirst)
            {
                return cacheFileFullPath;
            }
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            if (File.Exists(cacheFileFullPath))
            {
                if (cacheInfo.LastModify > DateTime.MinValue)
                {
                    request.IfModifiedSince = cacheInfo.LastModify;
                }
                if (!string.IsNullOrEmpty(cacheInfo.ETag))
                {
                    request.Headers.Add("If-None-Match", cacheInfo.ETag);
                }
            }
            else
            {
                var dirname = System.IO.Path.GetDirectoryName(cacheFileFullPath);
                System.IO.DirectoryEx.CreateDirectoryIfNotExist(dirname);//创建文件夹
            }
            try
            {
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    var etag = response.Headers["ETag"];
                    cacheInfo.ETag = etag;
                    cacheInfo.LastModify = response.LastModified;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            stream.WriteToFile(cacheFileFullPath);
                        }
                    }
                }
            }
            catch (WebException e)
            {
                var response = e.Response as HttpWebResponse;
                if (response.StatusCode != HttpStatusCode.NotModified)
                {
                    throw e;
                }
            }
            return cacheFileFullPath;
        }
        private Stream LoadFromRequest(Uri url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            var res = request.GetResponse();
            return res.GetResponseStream();
        }
        private CacheInfo GetCacheInfo(string url)
        {
            var r = cman.Get<CacheInfo>(url);
            return r.Item;
        }
    }


    class CacheInfo
    {
        public string Url { get; set; }
        public string ETag { get; set; }
        public string FileName { get; set; }
        public DateTime LastModify { get; set; }
    }
}
