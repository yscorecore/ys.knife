using System.IO;

namespace YS.Knife.Rest.Client.Resolves
{
    public class XmlResolver : IEntityResolver
    {
        public T Resolve<T>(string content)
        {
            var ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (StringReader sr = new StringReader(content))
            {
                return (T)ser.Deserialize(sr);
            }
        }
    }
}
