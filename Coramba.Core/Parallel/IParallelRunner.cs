using System.Threading.Tasks;

namespace Coramba.Core.Parallel
{
    public interface IParallelRunner
    {
        Task RunAsync(ParallelRunnerParameters parameters);
    }
}
