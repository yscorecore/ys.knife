using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace YS.Knife.Rest.Client.Resolves
{
    class AssemblyLoader
    {
        private static readonly IEntityResolver TextResolver = new TextResolver();
        private static readonly IEntityResolver JsonResolver = new JsonResolver();
        private static readonly IEntityResolver XmlResolver = new XmlResolver();
        // required net5
        [ModuleInitializer]

        public static void Initializer()
        {
            EntityResolver.Resolvers["text/plain"] = TextResolver;
            EntityResolver.Resolvers["text/html"] = TextResolver;
            EntityResolver.Resolvers["text/xml"] = XmlResolver;
            EntityResolver.Resolvers["application/xml"] = XmlResolver;
            EntityResolver.Resolvers["application/json"] = JsonResolver;
        }
    }
}
