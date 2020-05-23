using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace YS.Knife.Dictionary
{
    [TestClass]
    public class InjectDictionaryTest
    {
        [TestMethod]
        public void ShouldGetEmptyDictionaryWhenGetStringDictionary()
        {
            var provider = Utility.BuildProvider();
            var strDic = provider.GetService<IDictionary<string, string>>();
            Assert.IsNotNull(strDic);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldGetExceptionWhenGetIntDictionary()
        {
            var provider = Utility.BuildProvider();
            provider.GetService<IDictionary<int, IInterface1>>();
        }
        [TestMethod]
        public void ShouldGetDictionaryWhenGetInjectedClassDictionary()
        {
            var provider = Utility.BuildProvider();
            var strDic = provider.GetService<IDictionary<string, IInterface1>>();

            Assert.IsNotNull(strDic);
            Assert.IsTrue(strDic.ContainsKey(typeof(Class1).FullName));
            Assert.IsTrue(strDic.ContainsKey("c2"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ShouldGetExceptionWhenHasDuplicateKey()
        {
            var provider = Utility.BuildProvider((sc, c) =>
            {
                sc.AddSingleton<IInterface1, Class1>();
            });
            provider.GetService<IDictionary<string, IInterface1>>();
        }
    }

    public interface IInterface1
    {
    }

    [ServiceClass()]
    public class Class1 : IInterface1
    {
    }
    [ServiceClass()]
    [DictionaryKey("c2")]
    public class Class2 : IInterface1
    {
    }
}
