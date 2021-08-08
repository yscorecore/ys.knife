using FluentAssertions;
using Moq;
using Xunit;


namespace YS.Knife.Hosting
{

    public class InjectServiceTest : KnifeHost
    {
        [Inject]
        private readonly ITest test = Mock.Of<ITest>();

        [Inject]
        private ITest2 Prop { get; set; } = Mock.Of<ITest2>();

        [Fact]
        public void ShouldGetInjectServiceByCodeWhenDefineInjectAttributeInField()
        {
            this.GetService<ITest>().Should().NotBeNull();
        }
        [Fact]
        public void ShouldGetInjectServiceByCodeWhenDefineInjectAttributeInProperty()
        {
            this.GetService<ITest2>().Should().NotBeNull();
        }

        public interface ITest { }
        public interface ITest2 { }
    }
}
