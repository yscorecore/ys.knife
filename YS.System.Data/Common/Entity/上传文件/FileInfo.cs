using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace System.Data.Common
{
    /// <summary>
    /// 表示上传的文件信息
    /// </summary>
    public class FileInfo:BaseEntity<Guid>,ISequence
    {
        //public Guid FileID { get; set; }
        /// <summary>
        /// 表示文件的分类信息
        /// </summary>
        public Guid FileCatagory { get; set; }
        public int Sequence { get; set; }

        public string FileUrl { get; set; }
        /// <summary>
        /// 获取或设置文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 获取或设置文件大小,字节数
        /// </summary>
        public int FileSize { get; set; }
        /// <summary>
        /// 获取或设置文件的Hash
        /// </summary>
        public string FileHash { get; set; }
        /// <summary>
        /// 获取或设置文件的Hash类型
        /// </summary>
        public string HashType { get; set; }
        /// <summary>
        /// 获取或设置文件的MimeType
        /// </summary>
        public string MimeType { get; set; }
    }
}
