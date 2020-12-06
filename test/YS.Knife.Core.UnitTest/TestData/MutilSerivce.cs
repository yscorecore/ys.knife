

namespace YS.Knife.TestData
{
    [Multi]
    [Multi]
    [Multi2]
    [Multi2]
    public class MultiSerivce
    {

    }

    public class SubClass : MultiSerivce
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
