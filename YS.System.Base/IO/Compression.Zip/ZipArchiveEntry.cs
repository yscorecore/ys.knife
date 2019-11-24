using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO.Compression.Zip
{
    public class ZipArchiveEntry
    {
        public ZipArchive Archive
        {
            get
            {
                return null;
            }
        }
        public long CompressedLength
        {
            get
            {
                return 0L;
            }
        }
        public string FullName
        {
            get
            {
                return null;
            }
        }
        public DateTimeOffset LastWriteTime
        {
            get
            {
                return default(DateTimeOffset);
            }
            set
            {
            }
        }
        public long Length
        {
            get
            {
                return 0L;
            }
        }
        public string Name
        {
            get
            {
                return null;
            }
        }
        internal ZipArchiveEntry()
        {
        }
        public void Delete()
        {
        }
        public Stream Open()
        {
            return null;
        }
        public override string ToString()
        {
            return null;
        }
    }
}
