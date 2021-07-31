using System;
using PrimaryConstructor;
namespace YS.Knife.Generator.UnitTest
{
    [PrimaryConstructor]
    public partial class MainClass
    {
        [AutoNotify]
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
