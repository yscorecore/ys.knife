using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.UnitTest
{
    [TestClass]
    public class LimitInfoTest
    {
        [DataTestMethod]
        [DataRow("10", 0, 10)]
        [DataRow("1,10", 1, 10)]
        [DataRow(" 1 , 10 ", 1, 10)]
        public void ShouldParseSuccess(string limitStr, int offset, int limit)
        {
            var limitInfo = LimitInfo.Parse(limitStr);
            Assert.AreEqual(offset, limitInfo.Offset);
            Assert.AreEqual(limit, limitInfo.Limit);
        }
        [DataTestMethod]
        [DataRow("1 10")]
        [DataRow("a1,10")]
        [DataRow("1,10t")]
        [DataRow(" , ")]
        [ExpectedException(typeof(FilterInfoParseException))]
        public void ShouldParseFailure(string limitStr)
        {
            LimitInfo.Parse(limitStr);
        }



        [DataTestMethod]
        //[DataRow("10", 0, 10)]
        //[DataRow("1,10", 1, 10)]
        [DataRow(" 1 , 10 ", 1, 10)]
        public void ShouldConvertFromStringSuccess(string limitStr, int offset, int limit)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(LimitInfo));
            var canFromString = typeConverter.CanConvertFrom(typeof(string));
            Assert.AreEqual(true, canFromString);
            var limitInfo = typeConverter.ConvertFrom(limitStr) as LimitInfo;
            Assert.AreEqual(offset, limitInfo.Offset);
            Assert.AreEqual(limit, limitInfo.Limit);
        }
        [TestMethod]
        public void ShouldConvertToStringSuccess()
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(LimitInfo));
            var canToString = typeConverter.CanConvertTo(typeof(string));
            Assert.AreEqual(true, canToString);
            LimitInfo limitInfo = new LimitInfo(100, 20);
            Assert.AreEqual("100,20", typeConverter.ConvertTo(limitInfo, typeof(string)));
        }

        [DataTestMethod]
        [DataRow(1, 10, 0, 10)]
        [DataRow(2, 99, 99, 99)]
        public void ShouldParsePageInfoSuccess(int pageIndex, int pageSize, int offset, int limit)
        {
            var limitInfo = LimitInfo.FromPageInfo(pageIndex, pageSize);
            Assert.AreEqual(offset, limitInfo.Offset);
            Assert.AreEqual(limit, limitInfo.Limit);
        }
    }
}
