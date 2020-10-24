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
        public void ShouldGetExpectedExceptionWhenUseTemplateAndObject()
        {
            var exception = KnifeException.FromTemplate("001", "My name is {user}, I'm {age} years old.",
                new
                {
                    user = "zhangsan",
                    age = 12
                });
            Assert.AreEqual("001", exception.Code);
            Assert.AreEqual("My name is zhangsan, I'm 12 years old.", exception.Message);
            Assert.AreEqual(2, exception.Data.Count);
            Assert.IsTrue(exception.Data.Contains("user"));
            Assert.IsTrue(exception.Data.Contains("age"));
        }

        [TestMethod]
        public void ShouldGetExpectedExceptionWhenUseTemplateAndDictionary()
        {
            var exception = KnifeException.FromTemplate("001", "My name is {user}, I'm {age} years old.",
                new Dictionary<object, object>
                {
                    ["user"] = "zhangsan",
                    ["age"] = 12
                }
                );
            Assert.AreEqual("001", exception.Code);
            Assert.AreEqual("My name is zhangsan, I'm 12 years old.", exception.Message);
            Assert.AreEqual(2, exception.Data.Count);
            Assert.AreEqual("zhangsan", exception.Data["user"]);
            Assert.AreEqual(12, exception.Data["age"]);
        }
        [TestMethod]
        public void ShouldGetExpectedExceptionWhenUseTemplateAndArray()
        {
            var exception = KnifeException.FromTemplate("001", "My name is {0}, I'm {1} years old.",
                new object [] {"zhangsan",12 }
                );
            Assert.AreEqual("001", exception.Code);
            Assert.AreEqual("My name is zhangsan, I'm 12 years old.", exception.Message);
            Assert.AreEqual(2, exception.Data.Count);
            Assert.AreEqual("zhangsan", exception.Data["0"]);
            Assert.AreEqual(12, exception.Data["1"]);
        }

        [TestMethod]
        public void ShouldUseNullValuesWhenUseTemplateAndProvideTooLessValue()
        {
            var exception = KnifeException.FromTemplate("001",
                "My name is {user}, I'm {age} years old.", new Dictionary<object, object>
                {
                    ["user"] = "zhangsan"
                });
            Assert.AreEqual("001", exception.Code);
            Assert.AreEqual("My name is zhangsan, I'm (null) years old.", exception.Message);
            Assert.AreEqual(1, exception.Data.Count);
            Assert.AreEqual("zhangsan", exception.Data["user"]);
        }
        [TestMethod]
        public void ShouldIgnoreExtralValuesWhenUseTemplateAndProvideTooMuchValue()
        {
            var exception = KnifeException.FromTemplate("001",
                "My name is {user}, I'm {age} years old.", new Dictionary<string, object>
                {
                    ["user"] = "zhangsan",
                    ["age"] = 12,
                    ["tel"] = "123456"
                });
            Assert.AreEqual("001", exception.Code);
            Assert.AreEqual("My name is zhangsan, I'm 12 years old.", exception.Message);
            Assert.AreEqual(2, exception.Data.Count);
            Assert.AreEqual("zhangsan", exception.Data["user"]);
            Assert.AreEqual(12, exception.Data["age"]);
        }
    }
}
