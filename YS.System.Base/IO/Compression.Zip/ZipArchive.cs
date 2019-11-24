using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace System.IO.Compression.Zip
{
    public class ZipArchive : IDisposable
    {
        public ReadOnlyCollection<ZipArchiveEntry> Entries
        {
            get
            {
                return null;
            }
        }
        public ZipArchiveMode Mode
        {
            get
            {
                return ZipArchiveMode.Read;
            }
        }
        public ZipArchive(Stream stream)
        {
        }
        public ZipArchive(Stream stream, ZipArchiveMode mode)
        {
        }
        public ZipArchive(Stream stream, ZipArchiveMode mode, bool leaveOpen)
        {
        }
        public ZipArchiveEntry CreateEntry(string entryName)
        {
            return null;
        }
        public ZipArchiveEntry CreateEntry(string entryName, CompressionLevel compressionLevel)
        {
            return null;
        }
        public void Dispose()
        {
        }
        protected virtual void Dispose(bool disposing)
        {
        }
        public ZipArchiveEntry GetEntry(string entryName)
        {
            return null;
        }
    }
}
