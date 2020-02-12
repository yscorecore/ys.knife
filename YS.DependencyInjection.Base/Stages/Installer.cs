using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public abstract class Installer : IStageService
    {
        public string StageName => "install";

        public abstract Task Run(CancellationToken cancellationToken = default);
    }
}
