using FluentAssertions;
using Xunit;
namespace YS.Knife
{

    public class ReflectionExtensionsTest
    {
        [Fact]
        public void ShouldGetExpectedDefaultValue()
        {
            typeof(int).DefaultValue().Should().Be(0);
            typeof(int?).DefaultValue().Should().Be(null);
            typeof(object).DefaultValue().Should().Be(null);
        }
    }
}
