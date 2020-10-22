using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace YS.Knife.Common
{
    [TestClass]
    public class KnifeExceptionTest
    {
        [TestMethod]
        public void ShouldUseCodeValueWhenTemplateIsNull()
        {
            var exception = KnifeException.FromTemplate("001", null);
            Assert.AreEqual("001", exception.Code);
            Assert.AreEqual("001", exception.Message);
            Assert.AreEqual(0, exception.Data.Count);
        }

        [TestMethod]
        public void ShouldFormatMessageAndSetDataWhenUseTemplate()
        {
            var exception = KnifeException.FromTemplate("001", "My name is {user}, I'm {age} years old.", "zhangsan", 12);
            Assert.AreEqual("001", exception.Code);
            Assert.AreEqual("My name is zhangsan, I'm 12 years old.", exception.Message);
            Assert.AreEqual(2, exception.Data.Count);
            Assert.IsTrue(exception.Data.Contains("user"));
            Assert.IsTrue(exception.Data.Contains("age"));
        }

        [TestMethod]
        public void ShouldIgnoreExtraValuesWhenUseTemplateAndProvideTooMuchValues()
        {
            var exception = KnifeException.FromTemplate("001", "My name is {user}, I'm {age} years old.", "zhangsan", 12, "othervalues");
            Assert.AreEqual("001", exception.Code);
            Assert.AreEqual("My name is zhangsan, I'm 12 years old.", exception.Message);
            Assert.AreEqual(2, exception.Data.Count);
            Assert.AreEqual("zhangsan", exception.Data["user"]);
            Assert.AreEqual(12, exception.Data["age"]);
        }
        [TestMethod]
        public void ShouldUseNullValuesWhenUseTemplateAndProvideTooLessValue()
        {
            var exception = KnifeException.FromTemplate("001", "My name is {user}, I'm {age} years old.", "zhangsan");
            Assert.AreEqual("001", exception.Code);
            Assert.AreEqual("My name is zhangsan, I'm (null) years old.", exception.Message);
            Assert.AreEqual(2, exception.Data.Count);
            Assert.AreEqual("zhangsan", exception.Data["user"]);
            Assert.AreEqual(null, exception.Data["age"]);
        }
    }
}
