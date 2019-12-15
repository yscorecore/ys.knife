using Newtonsoft.Json;
using System.IO;

namespace System.Json
{
    public static  class JsonUtility
    {
        /// <summary>
        /// 将对象二进制序列化到指定的流中
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="stream"></param>
        public static void SerializeJsonToStream(this object obj, Stream stream)
        {
            if (obj == null) return;
            if (stream == null) throw new ArgumentNullException("stream");
            var text = JsonConvert.SerializeObject(obj);
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.Write(text);
            }
        }
        /// <summary>
        /// 将对象二进制序列化为字节数组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] SerializeJsonAsBytes(this object obj)
        {
            using (var ms = new MemoryStream())
            {
                SerializeJsonToStream(obj, ms);
                ms.Seek(0, SeekOrigin.Begin);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 将对象二进制序列化到文件中
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fileName"></param>
        public static void SerializeJsonToFile(this object obj, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            fileName = System.IO.Path.GetFullPath(fileName);
            string dir = System.IO.Path.GetDirectoryName(fileName);
            if (!IO.Directory.Exists(dir)) IO.Directory.CreateDirectory(dir);
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                SerializeJsonToStream(obj, file);
            }
        }
        /// <summary>
        /// 从指定的文件中以二进制的方式反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeSerializeJsonFromFile<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            fileName = Path.GetFullPath(fileName);
            using (var fs = File.OpenRead(fileName))
            {
                return DeserializeJsonFromStream<T>(fs);
            }
        }
        /// <summary>
        /// 从指定的字节数组中以二进制的方式反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bys"></param>
        /// <returns></returns>
        public static T DeserializeJsonFromBytes<T>(byte[] bys)
        {
            if (bys == null || bys.Length == 0) return default(T);
            using (var ms = new IO.MemoryStream(bys))
            {
                return DeserializeJsonFromStream<T>(ms);
            }
        }
        /// <summary>
        /// 从指定的字节数组中以二进制的方式反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bys"></param>
        /// <returns></returns>
        public static T DeserializeJsonFromBytes<T>(byte[] bys, int index, int count)
        {
            if (bys == null || bys.Length == 0) return default(T);
            using (var ms = new IO.MemoryStream(bys, index, count))
            {
                return DeserializeJsonFromStream<T>(ms);
            }
        }
        /// <summary>
        /// 从指定的流中以二进制的方式反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T DeserializeJsonFromStream<T>(Stream stream)
        {
            if (stream == null) return default(T);
            if (stream.Length == 0) return default(T);
            using (StreamReader sr = new StreamReader(stream))
            {
                var text = sr.ReadToEnd();
               return JsonConvert.DeserializeObject<T>(text);
            }
         
        }
    }
}
