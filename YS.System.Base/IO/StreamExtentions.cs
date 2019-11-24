using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
    public static class StreamExtentions
    {
        public static byte[] ToArray(this Stream stream)
        {
            if (stream is MemoryStream)
            {
                return ((MemoryStream)stream).ToArray();
            }
            else
            {
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    return memory.ToArray();
                }
            }
        }
        public static void WriteTo(this Stream input, Stream output, int bufferSize = 1024)
        {
            byte[] buffer = new byte[bufferSize];
            do
            {
                int count = input.Read(buffer, 0, bufferSize);
                if (count == 0) break;
                output.Write(buffer, 0, count);
            } while (true);
        }
        public static void WriteTo(this Stream input, Stream output, long maxLength, int bufferSize = 1024)
        {
            if (maxLength <= 0) return;

            byte[] buffer = new byte[bufferSize];
            long totalcount = 0;
            do
            {
                int readLength = (totalcount + bufferSize > maxLength) ? (int)(maxLength - totalcount) : bufferSize;
                int count = input.Read(buffer, 0, readLength);
                if (count == 0) break;
                output.Write(buffer, 0, count);
                totalcount += count;
            } while (true);
        }
        public static void WriteToFile(this Stream input, string outputFileName, int bufferSize = 1024)
        {
            using (var file = File.OpenWrite(outputFileName))
            {
                WriteTo(input, file, bufferSize);
            }
        }
        public static void WriteToFile(this Stream input, string outputFileName, long maxLength, int bufferSize = 1024)
        {
            using (var file = File.OpenWrite(outputFileName))
            {
                WriteTo(input, file, maxLength, bufferSize);
            }
        }
        public static string ReadAllText(this Stream stream)
        {

            using (var streamReader = new System.IO.StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
        public static string ReadAllText(this Stream stream, Encoding encoding)
        {
            using (var streamReader = new System.IO.StreamReader(stream, encoding))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}
