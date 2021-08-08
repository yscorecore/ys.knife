using System;
using System.ComponentModel;
using FluentAssertions;
using Xunit;
using YS.Knife.Data.Query;

namespace YS.Knife.Data.UnitTest
{

    public class LimitInfoTest
    {
        [Theory]
        [InlineData("10", 0, 10)]
        [InlineData("1,10", 1, 10)]
        [InlineData(" 1 , 10 ", 1, 10)]
        public void ShouldParseSuccess(string limitStr, int offset, int limit)
        {
            var limitInfo = LimitInfo.Parse(limitStr);
            limitInfo.Offset.Should().Be(offset);
            limitInfo.Limit.Should().Be(limit);
        }
        [Theory]
        [InlineData("1 10")]
        [InlineData("a1,10")]
        [InlineData("1,10t")]
        [InlineData(" , ")]
        public void ShouldParseFailure(string limitStr)
        {
            var action = new Action(() => LimitInfo.Parse(limitStr));
            action.Should().Throw<FilterInfoParseException>();
        }



        [Theory]
        //[InlineData("10", 0, 10)]
        //[InlineData("1,10", 1, 10)]
        [InlineData(" 1 , 10 ", 1, 10)]
        public void ShouldConvertFromStringSuccess(string limitStr, int offset, int limit)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(LimitInfo));
            var canFromString = typeConverter.CanConvertFrom(typeof(string));
            canFromString.Should().Be(true);
            var limitInfo = typeConverter.ConvertFrom(limitStr) as LimitInfo;
            limitInfo.Offset.Should().Be(offset);
            limitInfo.Limit.Should().Be(limit);
        }
        [Fact]
        public void ShouldConvertToStringSuccess()
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(LimitInfo));
            var canToString = typeConverter.CanConvertTo(typeof(string));
            canToString.Should().Be(true);
            LimitInfo limitInfo = new LimitInfo(100, 20);
            typeConverter.ConvertTo(limitInfo, typeof(string)).Should().Be("100,20");
        }

        [Theory]
        [InlineData(1, 10, 0, 10)]
        [InlineData(2, 99, 99, 99)]
        public void ShouldParsePageInfoSuccess(int pageIndex, int pageSize, int offset, int limit)
        {
            var limitInfo = LimitInfo.FromPageInfo(pageIndex, pageSize);
            limitInfo.Offset.Should().Be(offset);
            limitInfo.Limit.Should().Be(limit);
        }
    }
}
