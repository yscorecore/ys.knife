using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace System
{
    /// <summary>
    /// 表示元数据成员信息关联资源的基类
    /// </summary>
    public abstract class ResourceAttribute : Attribute
    {
        public ResourceAttribute()
        {

        }
        public ResourceAttribute(string id)
        {
            this._id = id;
        }
        private string _id;
        /// <summary>
        /// 资源的标示名称
        /// </summary>
        public string Id
        {
            get { return _id ?? string.Empty; }
            set { _id = value; }
        }
        public abstract Stream GetStream(MemberInfo memberinfo);
      //  public abstract string GetString(MemberInfo memberinfo);
        public abstract string GetString(MemberInfo memberinfo,Encoding encoding);
        public static ResourceAttribute GetResourceAttribute(MemberInfo memberinfo, string id = "")
        {
            id = id ?? string.Empty;
            if (!caches.ContainsKey(memberinfo))
            {
                caches.Add(memberinfo, Attribute.GetCustomAttributes(memberinfo, typeof(ResourceAttribute), true) as ResourceAttribute[]);
            }
            var att = (from p in caches[memberinfo] where p.Id == id select p).FirstOrDefault();
            return att;
        }
        public static Stream GetResourceStream(MemberInfo memberinfo, string id = "")
        {
            ResourceAttribute att = GetResourceAttribute(memberinfo, id);
            return att == null ? null : att.GetStream(memberinfo);
        }
        public static string GetResourceString(MemberInfo memberinfo, string id ,Encoding encoding)
        {
            ResourceAttribute att = GetResourceAttribute(memberinfo, id);
            return att == null ? null : att.GetString(memberinfo,encoding);
        }
        private static Dictionary<MemberInfo, ResourceAttribute[]> caches = new Dictionary<MemberInfo, ResourceAttribute[]>();
        public static bool DefinedResource(MemberInfo member)
        {
            if (caches.ContainsKey(member))
            {
                return caches[member].Length > 0;
            }
            else
            {
                if (Attribute.IsDefined(member, typeof(ResourceAttribute)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }

    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class FileResourceAttribute : ResourceAttribute
    {
        /// <summary>
        /// 相对路径或绝对路径
        /// </summary>
        public virtual string FilePath { get; private set; }
        public virtual string FileFullPath
        {
            get 
            {
                return System.IO.PathEx.GetFullPath(this.FilePath);
            }
        }
        public FileResourceAttribute(string id, string filePath)
            : base(id)
        {
            this.FilePath = filePath;
        }
        public FileResourceAttribute(string filePath)
        {
            this.FilePath = filePath;
        }
        public override Stream GetStream(MemberInfo memberinfo)
        {
            if (File.Exists(this.FileFullPath))
            {
                return new FileStream(this.FileFullPath, FileMode.Open);
            }
            else
            {
                return null;
            }
        }

        public override string GetString(MemberInfo memberinfo, Encoding encoding)
        {
            if (File.Exists(this.FileFullPath))
            {
                return System.IO.File.ReadAllText(this.FileFullPath, encoding);

            }
            else
            {
                return null;
            }
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ManifestResourceAttribute : ResourceAttribute
    {
        readonly string resourceName;
        readonly string resourceTypeFullName;
        readonly string resourceAssembly;//表示资源所在的程序集

        public string AssemblyFullName
        {
            get { return resourceAssembly; }
        } 

        #region 构造函数
        [Obsolete("please use the constructor which contains the 'id' argument")]
        public ManifestResourceAttribute(string resourceName)
            : base("")
        {
            this.resourceName = resourceName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestResourceAttribute" /> class.
        /// </summary>
        /// <param name="tyName">The type whose namespace is used to scope the manifest resource name.</param>
        /// <param name="resourceName">Name of the resource.</param>
        [Obsolete("please use the constructor which contains the 'id' argument")]
        public ManifestResourceAttribute(Type tyName, string resourceName)
            : this("", resourceName)
        {
            if (tyName != null)
            {
                this.resourceTypeFullName = tyName.AssemblyQualifiedName;
            }
        }
        #endregion

        #region 构造函数2
        public ManifestResourceAttribute(string id, string resourceName)
            : base(id)
        {
            this.resourceName = resourceName;
        }
        public ManifestResourceAttribute(string id, string resourceAssembly, string resourceName)
            : base(id)
        {
            this.resourceAssembly = resourceAssembly;
            this.resourceName = resourceName;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ManifestResourceAttribute" /> class.
        /// </summary>
        /// <param name="tyName">The type whose namespace is used to scope the manifest resource name.</param>
        /// <param name="resourceName">Name of the resource.</param>
        public ManifestResourceAttribute(string id, Type tyName, string resourceName)
            : base(id)
        {
            this.resourceName = resourceName;
            if (tyName != null)
            {
                this.resourceTypeFullName = tyName.AssemblyQualifiedName;
            }
        }
        #endregion
        public string ResourceName
        {
            get { return resourceName; }
        }
        //public string ResourceTypeFullName
        //{
        //    get { return this.resourceTypeFullName; }
        //}
        internal Type ResourceType
        {
            get
            {
                if (string.IsNullOrEmpty(this.resourceTypeFullName))
                {
                    return null;
                }
                else
                {
                    return Type.GetType(this.resourceTypeFullName);
                }
            }
        }
        internal Assembly GetResourceAssembly(MemberInfo memberinfo)
        {
            if (!string.IsNullOrEmpty(resourceAssembly))
            {
                return Assembly.Load(this.resourceAssembly);
            }
            else
            {
                return memberinfo.DeclaringType == null ? (memberinfo as Type).Assembly : memberinfo.DeclaringType.Assembly;
            }
        }
        //public static bool DefinedResource(MemberInfo member)
        //{
        //    if(Attribute.IsDefined(member,typeof(ManifestResourceAttribute)))
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        //public static Stream GetResourceStream(MemberInfo member)
        //{
        //    if(Attribute.IsDefined(member,typeof(ManifestResourceAttribute)))
        //    {
        //        ManifestResourceAttribute res = Attribute.GetCustomAttribute(member,typeof(ManifestResourceAttribute)) as ManifestResourceAttribute;
        //        Type ty = res.ResourceType;
        //        if(ty != null)
        //        {
        //            return member.Module.Assembly.GetManifestResourceStream(ty,res.ResourceName);
        //        }
        //        else
        //        {
        //            return member.Module.Assembly.GetManifestResourceStream(res.ResourceName);
        //        }
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}


        public override Stream GetStream(MemberInfo memberinfo)
        {
            Type ty = this.ResourceType;
            if (ty != null)
            {
                return ty.Assembly.GetManifestResourceStream(ty, resourceName);
            }
            else
            {
               return this.GetResourceAssembly(memberinfo).GetManifestResourceStream(resourceName);
               
            }
        }

        public override string GetString(MemberInfo memberinfo,Encoding encoding)
        {
            using (var stream = this.GetStream(memberinfo))
            {
                if (stream!=null)
                {
                    StreamReader sr = new StreamReader(stream, encoding);
                    return sr.ReadToEnd();
                }
                else
                {
                    return null;
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ManagedResourceAttribute : ResourceAttribute
    {
        public ManagedResourceAttribute(string id, Type resourceSource, string key)
            : base(id)
        {
            this.AssemblyFullName = resourceSource.Assembly.FullName;
            this.BaseName = resourceSource.FullName;
            this.Key = key;
        }
        public ManagedResourceAttribute(string id, Type assemblyFinderType, string baseName, string key)
            : base(id)
        {
            this.AssemblyFullName = assemblyFinderType.Assembly.FullName;
            this.BaseName = BaseName;
            this.Key = key;
        }
        public ManagedResourceAttribute(string id, string baseName, string key)
            : base(id)
        {
            //this.AssemblyFullName = assemblyFinderType.Assembly.FullName;
            this.BaseName = BaseName;
            this.Key = key;
        }
        public ManagedResourceAttribute(string id, string assemblyFullName, string baseName, string key)
            : base(id)
        {
            this.AssemblyFullName = assemblyFullName;
            this.BaseName = BaseName;
            this.Key = key;
        }
        public string AssemblyFullName { get; private set; }
        public string BaseName { get; private set; }
        public string Key { get; set; }
        internal Assembly GetResourceAssembly(MemberInfo memberinfo)
        {
            if (!string.IsNullOrEmpty(AssemblyFullName))
            {
                return Assembly.Load(AssemblyFullName);
            }
            else
            {
                return memberinfo.DeclaringType == null ? (memberinfo as Type).Assembly : memberinfo.DeclaringType.Assembly;
            }
        }
        public override Stream GetStream(MemberInfo memberinfo)
        {
            return ResourceManagerFactory.GetResourceManager(GetResourceAssembly(memberinfo),
                BaseName).GetStream(Key);
        }
        public override string GetString(MemberInfo memberinfo, Encoding encoding)
        {
            return ResourceManagerFactory.GetResourceManager(GetResourceAssembly(memberinfo),
              BaseName).GetString(Key);
        }
        public  object GetObject(MemberInfo memberinfo)
        {
            return ResourceManagerFactory.GetResourceManager(GetResourceAssembly(memberinfo),
             BaseName).GetObject(Key);
        }
    }

    public static class ResourceAttributeEx
    {
        public static string GetResourceString(this MemberInfo member, string id = "")
        {
            return ResourceAttribute.GetResourceString(member, id, Encoding.UTF8);
        }
        public static string GetResourceString(this MemberInfo member, string id, Encoding encoding)
        {
            return ResourceAttribute.GetResourceString(member, id, encoding);           
        }
        public static Stream GetResourceStream(this MemberInfo member, string id = "")
        {
            return ResourceAttribute.GetResourceStream(member, id);
        }
        public static Stream GetResourceStream(this MemberInfo member, string id, Encoding encoding)
        {
            return ResourceAttribute.GetResourceStream(member, id);
        }
        public static string GetFileResourcePath(this MemberInfo member, string id="")
        {
            var att = ResourceAttribute.GetResourceAttribute(member, id);
            if (att is FileResourceAttribute)
            {
                return (att as FileResourceAttribute).FileFullPath;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
