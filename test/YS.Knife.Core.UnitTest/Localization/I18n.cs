using Microsoft.Extensions.Localization;
using YS.Knife.Aop;
namespace YS.Knife.Localization
{
    [StringResources]
    public interface I18n
    {
        [SR(nameof(Title), "This is the Title.")]
        string Title{get;}

        [SR(nameof(Hello), "Hello,World")]
        string Hello();

        [SR(nameof(SayHelloWithIndex), "Hello, I'm {0}, I'm {1:d3} years old.")]
        string SayHelloWithIndex(string name, int age);
        
        [SR(nameof(SayHelloWithName), "Hello, I'm {name}, I'm {age:d3} years old.")]
        string SayHelloWithName(int age, string name);

        [SR(nameof(SayHelloWithName), "Hello, I'm {1}, I'm {age:d3} years old.")]
        string SayHelloWithNameAndIndex(int age, string name);
    }
}