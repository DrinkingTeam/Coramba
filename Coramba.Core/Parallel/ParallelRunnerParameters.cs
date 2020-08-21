using System.Collections.Generic;
using System.Threading;

namespace Coramba.Core.Parallel
{
    public class ParallelRunnerParameters
    {
        public List<ParallelRunnerTaskInfo> Tasks { get; } = new List<ParallelRunnerTaskInfo>();
        public int MaxConcurrency { get; set; } = 10;
        public CancellationToken CancellationToken { get; set; } = default;
    }
}
