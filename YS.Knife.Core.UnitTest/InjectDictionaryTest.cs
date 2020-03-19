using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace YS.Knife
{
    [TestClass]
    public class InjectDictionaryTest
    {
        [TestMethod]
        public void ShouldGetEmptyDictionaryWhenGetStringDictionaryService()
        {
            var provider = Utility.BuildProvider();
            var strDic = provider.GetService<IDictionary<string, string>>();
            Assert.IsNotNull(strDic);
        }

    }
}
