using System.Threading;
using System.Threading.Tasks;

namespace System
{
    public interface IStageService
    {
        string StageName { get; }
        Task Run(CancellationToken cancellationToken = default);
    }
}
