

namespace YS.Knife.TestData
{
    [Multi]
    [Multi]
    [Multi2]
    [Multi2]
    public class MultiService
    {

    }

    public class SubClass : MultiService
    {
    }
    internal class OutterClass
    {
        [Multi]
        [Multi]
        [Multi2]
        [Multi2]
        internal class InnerClass
        {

        }
    }
}
