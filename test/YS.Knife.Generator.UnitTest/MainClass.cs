using System;

namespace YS.Knife.Generator.UnitTest
{
    public partial class MainClass
    {
        //[YS.Knife.Autowired]
        private readonly DependClass _depend;
    }

    public partial class DependClass
    {
        
        private readonly DependClass2 _depend;
    }

    public class DependClass2
    {
    }
}
