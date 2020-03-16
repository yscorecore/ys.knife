using System.Threading;
using System.Threading.Tasks;

namespace YS.Knife
{
    public interface IStageService
    {
        string StageName { get; }
        Task Run(CancellationToken cancellationToken = default);
    }
}
