using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Runtime.Serialization
{
    public static class BinUtility
    {
        /// <summary>
        /// 将对象二进制序列化到指定的流中
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        public static void SerializeBinToStream(this object obj, Stream stream)
        {
            if (obj == null) return;
            if (stream == null) throw new ArgumentNullException("stream");
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            bf.Serialize(stream, obj);
        }
        /// <summary>
        /// 将对象二进制序列化为字节数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeBinAsBytes(this object obj)
        {
            using (var ms = new MemoryStream())
            {
                SerializeBinToStream(obj, ms);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 将对象二进制序列化到文件中
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fileName"></param>
        public static void SerializeBinToFile(this object obj, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            fileName = System.IO.PathEx.GetFullPath(fileName);
            string dir = System.IO.Path.GetDirectoryName(fileName);
            if (!IO.Directory.Exists(dir)) IO.Directory.CreateDirectory(dir);
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                SerializeBinToStream(obj, file);
            }
        }
        /// <summary>
        /// 从指定的文件中以二进制的方式反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeSerializeBinFromFile<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            fileName = PathEx.GetFullPath(fileName);
            using (var fs = File.OpenRead(fileName))
            {
                return DeserializeBinFromStream<T>(fs);
            }
        }
        /// <summary>
        /// 从指定的字节数组中以二进制的方式反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bys"></param>
        /// <returns></returns>
        public static T DeserializeBinFromBytes<T>(byte[] bys)
        {
            if (bys == null || bys.Length == 0) return default(T);
            using (var ms = new IO.MemoryStream(bys))
            {
                return DeserializeBinFromStream<T>(ms);
            }
        }
        /// <summary>
        /// 从指定的字节数组中以二进制的方式反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bys"></param>
        /// <returns></returns>
        public static T DeserializeBinFromBytes<T>(byte[] bys, int index, int count)
        {
            if (bys == null || bys.Length == 0) return default(T);
            using (var ms = new IO.MemoryStream(bys, index, count))
            {
                return DeserializeBinFromStream<T>(ms);
            }
        }
        /// <summary>
        /// 从指定的流中以二进制的方式反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T DeserializeBinFromStream<T>(Stream stream)
        {
            if (stream == null) return default(T);
            if (stream.Length == 0) return default(T);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (T)bf.Deserialize(stream);
        }
    }
}
