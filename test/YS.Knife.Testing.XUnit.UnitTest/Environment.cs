using System;
using System.Threading.Tasks;
using Xunit;

namespace YS.Knife.Testing.XUnit.UnitTest
{
    [CollectionDefinition(nameof(Environment))]
    public class Environment : IDisposable, ICollectionFixture<Environment>
    {
        public static int Counter = 0;
        public Environment()
        {
            this.OnSetUp();
        }

        public void Dispose()
        {
            this.OnTeardown();
        }

        private void OnSetUp()
        {
            Counter++;
        }

        private void OnTeardown()
        {
            Counter--;
        }


    }
}
