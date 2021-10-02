using System;
using YS.Knife;

namespace SourceGenerator.Singleton
{
    [SingletonPattern]
    public partial class Class1
    {
        public string Name { get; set; } = "knife";
    }

    [SingletonPattern]
    public partial class Class2<T>
    {
        public string Name { get; set; } = typeof(T).FullName;
    }
    public class Class3
    {
        public void Hello()
        {

            Console.WriteLine(Class1.Instance.Name);
            Console.WriteLine(Class2<int>.Instance.Name);
        }
    }


}
