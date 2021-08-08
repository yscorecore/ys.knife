using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace YS.Knife.Dictionary
{

    public class InjectDictionaryTest
    {
        [Fact]
        public void ShouldGetEmptyDictionaryWhenGetStringDictionary()
        {
            var provider = Utility.BuildProvider();
            var strDic = provider.GetService<IDictionary<string, string>>();

            strDic.Should().NotBeNull();
        }

        [Fact]
        public void ShouldGetExceptionWhenGetIntDictionary()
        {
            var provider = Utility.BuildProvider();
            var action = new Action(() => { provider.GetService<IDictionary<int, IInterface1>>(); });
            action.Should().Throw<InvalidOperationException>();
        }
        [Fact]
        public void ShouldGetDictionaryWhenGetInjectedClassDictionary()
        {
            var provider = Utility.BuildProvider();
            var strDic = provider.GetService<IDictionary<string, IInterface1>>();

            strDic.Should().NotBeNull();
            strDic.Should().ContainKey(typeof(Class1).FullName);
            strDic.Should().ContainKey("c2");
        }

        [Fact]
        public void ShouldGetExceptionWhenHasDuplicateKey()
        {
            var provider = Utility.BuildProvider((sc, c) =>
            {
                sc.AddSingleton<IInterface1, Class1>();
            });


            var action = new Action(() => provider.GetService<IDictionary<string, IInterface1>>());
            action.Should().Throw<InvalidOperationException>();
        }
    }

    public interface IInterface1
    {
    }

    [Service()]
    public class Class1 : IInterface1
    {
    }
    [Service()]
    [DictionaryKey("c2")]
    public class Class2 : IInterface1
    {
    }
}
