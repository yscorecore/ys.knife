namespace System
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Resources;
    /// <summary>
    /// 表示类型成员的友好名称
    /// </summary>
    [AttributeUsage(AttributeTargets.All,AllowMultiple = false,Inherited = true)]
    public class DisplayTextAttribute:Attribute
    {
        private string m_text;
        /// <summary>
        /// 初始化 <see cref="DisplayTextAttribute"/> 的新实例。
        /// </summary>
        /// <param name="text">显示文本</param>
        public DisplayTextAttribute(string val)
        {
            this.m_text = val;
        }
        /// <summary>
        /// 获取或设置显示文本
        /// </summary>
        public virtual string Text
        {
            get
            {
                return this.m_text;
            }
            set
            {
                this.m_text = value;
            }
        }
    }
    /// <summary>
    /// 提供类型成员名称的友好名称的转换功能
    /// </summary>
    /// <seealso cref="DisplayTextAttribute"/>
    public sealed class DisplayText:IMemberTranslate<string>
    {
        static Dictionary<MemberInfo, DisplayTextAttribute> Cache = new Dictionary<MemberInfo, DisplayTextAttribute>();
        static object locker = new object();
        static DisplayText instance;
        private DisplayText()
        {
        }
        public static DisplayText Instance
        {
            get {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new DisplayText();
                        }
                    }
                }
                return instance;
            }
        }
        public string GetTranslateValue(MemberInfo memberInfo)
        {
            if(!Cache.ContainsKey(memberInfo))
            {
                object[] obj = memberInfo.GetCustomAttributes(typeof(DisplayTextAttribute), true);
                DisplayTextAttribute att = obj == null || obj.Length == 0 ? null : obj[0] as DisplayTextAttribute;
                Cache.Add(memberInfo, att);
            }
            DisplayTextAttribute att2 = Cache[memberInfo];
            if (att2 != null)
            {
                string text = att2.Text;
                return string.IsNullOrEmpty(text) ? memberInfo.Name : text;
            }
            else
            {
                return memberInfo.Name;
            }

        }
    }
    /// <summary>
    /// 提供从托管资源翻译元数据的基类
    /// </summary>
    public abstract class ResourceMemberTranslateBase<T>:IMemberTranslate<T>
    {
        #region 构造函数
        public ResourceMemberTranslateBase()
        {

        }
        public ResourceMemberTranslateBase(ResourceManager resourceManager)
        {
            if (resourceManager == null) throw new ArgumentNullException("resourceManager");
            this.ResourceManager = resourceManager;
        }
        public ResourceMemberTranslateBase(ResourceManager resourceManager, CultureInfo cultureInfo)
        {
            if (resourceManager == null) throw new ArgumentNullException("resourceManager");
            this.ResourceManager = resourceManager;
            this.CultureInfo = cultureInfo;
        }
        #endregion

        //public virtual Assembly ResourceAssembly { get; private set; }

        //public virtual string ResourceBaseName { get; private set; }

        public virtual ResourceManager ResourceManager { get; protected set; }



        public virtual CultureInfo CultureInfo { get; set; }

        public virtual T GetTranslateValue(MemberInfo memberInfo)
        {
           // var ass=(memberInfo is Type)?(memberInfo as Type).Assembly :memberInfo.DeclaringType.Assembly
            if (ResourceManager == null) throw new ApplicationException("the ResourceManager should not be null");
            if(memberInfo == null) throw new ArgumentNullException("memberInfo");
            string key=this.GetResourceKey(memberInfo);
            return GetTranslateValueByResourceKey(this.ResourceManager, this.CultureInfo, memberInfo, key);
        }
        protected virtual string GetResourceKey(MemberInfo memberInfo)
        {
            if(memberInfo is Type)
            {
                return memberInfo.ReflectedType.FullName.Replace('.','_');
            }
            else
            {
                return string.Format("{0}__{1}",memberInfo.ReflectedType.FullName.Replace('.','_'),memberInfo.Name);
            }
        }
        protected abstract T GetTranslateValueByResourceKey(ResourceManager resourceManager, CultureInfo cultureInfo, MemberInfo memberInfo,string key);

    }
    /// <summary>
    /// 表示从托管资源中提供元数据的友好名称的类
    /// </summary>
    public class ManagedResourceDisplayTextProvider : ResourceMemberTranslateBase<string>
    {
        public ManagedResourceDisplayTextProvider():base()
        {

        }

        public ManagedResourceDisplayTextProvider(Assembly assembly, string baseName):base(ResourceManagerFactory.GetResourceManager(assembly,baseName))
        { 
        
        }
        public ManagedResourceDisplayTextProvider(ResourceManager resourceManager):base(resourceManager)
        {
           
        }
        public ManagedResourceDisplayTextProvider(ResourceManager resourceManager, CultureInfo cultureInfo):base(resourceManager,cultureInfo)
        {
          
        }
        protected override string GetTranslateValueByResourceKey(ResourceManager resourceManager, CultureInfo cultureInfo, MemberInfo memberInfo, string key)
        {
            if (resourceManager == null)
            {
                return memberInfo.Name;
            }
            else
            {
              return resourceManager.GetString(key, cultureInfo);
            }
        }
    }
    /// <summary>
    /// 提供获取元数据关联信息的方法
    /// </summary>
    public static class MemberTranslateEx
    {
        /// <summary>
        /// 获取成员的友好名称
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public static string GetDisplayText(this MemberInfo memberInfo)
        {
            return DisplayText.Instance.GetTranslateValue(memberInfo);
        }
        public static string GetDisplayText(this Enum item)
        {
            var members = item.GetEnumMemberInfos();
            string[] res = new string[members.Length];
            for (int i = 0; i < res.Length;i++ )
            {
                res[i] = DisplayText.Instance.GetTranslateValue(members[i]);
            }
            return string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, res);
        }
        /// <summary>
        /// 获取成员的友好名称
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="translate"></param>
        /// <returns></returns>
        public static string GetDisplayText(this MemberInfo memberInfo, IMemberTranslate<string> translate)
        {
            if (translate == null) translate = DisplayText.Instance;
            return translate.GetTranslateValue(memberInfo); 
        }
        public static string GetDisplayText(this Enum item,IMemberTranslate<string> translate)
        {
            if (translate == null) translate = DisplayText.Instance;
            var members = item.GetEnumMemberInfos();
            string[] res = new string[members.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = translate.GetTranslateValue(members[i]);
            }
            return string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, res);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo"></param>
        /// <param name="translate"></param>
        /// <returns></returns>
        public static T GetTranslateValue<T>(this MemberInfo memberInfo, IMemberTranslate<T> translate)
        {
            return translate.GetTranslateValue(memberInfo);
        }
    }
}
