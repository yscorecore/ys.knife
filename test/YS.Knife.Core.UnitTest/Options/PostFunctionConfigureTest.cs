using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using YS.Knife.Options;
using System.Linq;
namespace YS.Knife.Core.UnitTest.Options
{
    [TestClass]
    public class PostFunctionConfigureTest
    {

        [TestMethod]
        public void ShouldReverseFunctionInputWhenDefineReverseFunction()
        {
            var provider = Utility.BuildProvider();
            var options = provider.GetService<IOptions<FunctionTestOptions>>();
            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);
            Assert.AreEqual("Hello,dlroW.", options.Value.HasReverse);
        }

        [TestMethod]
        public void ShouldKeepOriginInputWhenNotDefineReverse2Function()
        {
            var provider = Utility.BuildProvider();
            var options = provider.GetService<IOptions<FunctionTestOptions>>();
            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);
            Assert.AreEqual("Hello,${{reverse2(World)}}.", options.Value.HasReverse2);
        }

        [TestMethod]
        public void ShouldEachFunctionInvokedWhenDefineMuipleFunction()
        {
            var provider = Utility.BuildProvider();
            var options = provider.GetService<IOptions<FunctionTestOptions>>();
            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);
            Assert.AreEqual("hello,WORLD.", options.Value.MutilFunction);
        }

        [TestMethod]
        public void ShouldNestedTypeFunctionInvokedWhenNestedObjectDefineMuipleFunction()
        {
            var provider = Utility.BuildProvider();
            var options = provider.GetService<IOptions<FunctionTestOptions>>();
            Assert.IsNotNull(options);
            Assert.IsNotNull(options.Value);
            Assert.AreEqual("hello", options.Value.Nested.Lower);
        }
    }
    [OptionsClass]
    public class FunctionTestOptions
    {
        public string HasReverse { get; set; } = "Hello,${{reverse(World)}}.";
        public string HasReverse2 { get; set; } = "Hello,${{reverse2(World)}}.";
        public string MutilFunction { get; set; } = "${{lower(hello,)}}${{upper(World)}}.";

        public NestedOptions Nested { get; set; } = new NestedOptions();
    }
    public class NestedOptions
    {
        public string Lower { get; set; } = "${{lower(Hello)}}";
    }
    [OptionsPostFunction]
    [DictionaryKey("reverse")]
    public class ReverseCaseFunction : IOptionsPostFunction
    {
        public string Invoke(string context)
        {
            return new string(context.ToCharArray().Reverse().ToArray());
        }
    }
}
