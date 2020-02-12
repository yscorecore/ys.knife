using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public abstract class UnInstaller : IStageService
    {
        public string StageName => "install";

        public abstract Task Run(CancellationToken cancellationToken = default);
    }
}
