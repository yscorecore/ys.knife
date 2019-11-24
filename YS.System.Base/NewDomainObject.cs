using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示在新的应用程序欲里面开的远程对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NewDomainObject<T> : IDisposable
         where T : MarshalByRefObject
    {
        public AppDomain Domain { get; private set; }
        public NewDomainObject(string domainName)
        {
            Domain = AppDomain.CreateDomain(domainName);
            //.net Standand 不支持 CreateInstanceAndUnwrap
            //this.RemoteObject = (T)Domain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName, typeof(T).FullName);
        }

        public T RemoteObject { get; private set; }

        #region IDisposable 成员

        public void UnLoad()
        {
            AppDomain.Unload(Domain);
        }
        public void Dispose()
        {
            AppDomain.Unload(Domain);
        }

        #endregion
    }
}
