using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace YS.Knife.Options
{

    public class OptionsTest
    {
        [Fact]
        public void ShouldNotGetNullWhenNotDefineOptionsAttribute()
        {
            var provider = Utility.BuildProvider();
            var options = provider.GetService<IOptions<Custom0Options>>();
            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.Value.Should().Be(default);
        }

        [Fact]
        public void ShouldGetConfigedValueWhenDefineOptionsAttributeWithEmptyConfigKey()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["Custom1:Value"] = "some_value"
            });
            var options = provider.GetService<IOptions<Custom1Options>>();
            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.Value.Should().Be("some_value");
        }

        [Fact]
        public void ShouldGetConfigedValueWhenDefineOptionsAttributeConfigKey()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["C2:Value"] = "some_value"
            });
            var options = provider.GetService<IOptions<Custom2Options>>();
            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.Value.Should().Be("some_value");
        }

        [Fact]
        public void ShouldThrowExceptionWhenDefineDataAnnotationsAndConfigInvalidValue()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["Custom7:Value"] = "not a url value"
            });
            var options = provider.GetService<IOptions<Custom7Options>>();
            options.Should().NotBeNull();
            new Action(() => { _ = options.Value; }).Should().Throw<OptionsValidationException>();
        }

        [Fact]
        public void ShouldGetExpectedValueWhenDefineDataAnnotationsAndConfigValidValue()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["Custom7:Value"] = "http://localhost"
            });
            var options = provider.GetService<IOptions<Custom7Options>>();
            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.Value.Should().Be("http://localhost");
        }

        [Fact]
        public void ShouldGetConfigedValueWhenDefineOptionsAttributeConfigKeyIsNested()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["C:B:D:Value"] = "some_value"
            });
            var options = provider.GetService<IOptions<Custom3Options>>();
            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.Value.Should().Be("some_value");
        }
        [Fact]
        public void ShouldGetConfigedValueWhenDefineOptionsAttributeConfigKeyIsNestedWithDot()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["C:B:D:Value"] = "some_value"
            });
            var options = provider.GetService<IOptions<Custom4Options>>();
            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.Value.Should().Be("some_value");
        }
        [Fact]
        public void ShouldGetConfigedValueWhenDefineOptionsAttributeConfigKeyIsNestedWithDoubleUnderScore()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["C:B:D:Value"] = "some_value"
            });
            var options = provider.GetService<IOptions<Custom5Options>>();
            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.Value.Should().Be("some_value");
        }

        [Fact]
        public void ShouldGetConfigedValueWhenDefineOptionsAttributeConfigKeyIsEmptyString()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["Value"] = "some_value"
            });
            var options = provider.GetService<IOptions<Custom6Options>>();
            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.Value.Should().Be("some_value");
        }

        [Fact]
        public void ShouldThrowExceptionWhenDefineCustomValidateAndConfigInvalidValue()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["Custom8:Number"] = "1"
            });
            var options = provider.GetService<IOptions<Custom8Options>>();
            options.Should().NotBeNull();
            new Action(() => { _ = options.Value; }).Should().Throw<OptionsValidationException>();
        }
        [Fact]
        public void ShouldGetConfigedValueWhenDefineCustomValidateAndConfigValidValue()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["Custom8:Number"] = "2"
            });
            var options = provider.GetService<IOptions<Custom8Options>>();
            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
        }

        [Fact]
        public void ShouldGetPostedValueWhenDefineOptionsAttributeAndPostHandler()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["Custom9:Text"] = "some_text"
            });
            var options = provider.GetService<IOptions<Custom9Options>>();
            options.Should().NotBeNull();
            options.Value.Should().NotBeNull();
            options.Value.Text.Should().Be("__some_text");
        }

        [Fact]
        public void ShouldThrowExceptionWhenNestedObjectConfigInvalidValue()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["DeepObject:Address:Province:Code"] = "invalidValue"
            });
            var action = new Action(() => { _ = provider.GetService<IOptions<DeepObjectOptions>>().Value; });
            action.Should().Throw<OptionsValidationException>();
        }

        [Fact]
        public void ShouldThrowExceptionWhenNestedListConfigInvalidValue()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["DeepList:Addresses:0:Province:Code"] = "invalidValue"
            });
            var action = new Action(() => { _ = provider.GetService<IOptions<DeepListOptions>>().Value; });
            action.Should().Throw<OptionsValidationException>();

        }

        [Fact]
        public void ShouldThrowExceptionWhenNestedDicConfigInvalidValue()
        {
            var provider = Utility.BuildProvider(new Dictionary<string, string>
            {
                ["DeepDic:Addresses:name:Province:Code"] = "invalidValue"
            });
            var action = new Action(() => { _ = provider.GetService<IOptions<DeepDicOptions>>().Value; });
            action.Should().Throw<OptionsValidationException>();
        }
    }
}
