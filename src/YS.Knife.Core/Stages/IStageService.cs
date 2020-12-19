using System.Threading;
using System.Threading.Tasks;

namespace YS.Knife.Stages
{
    public interface IStageService
    {
        Task Run(CancellationToken cancellationToken = default);
    }
}
