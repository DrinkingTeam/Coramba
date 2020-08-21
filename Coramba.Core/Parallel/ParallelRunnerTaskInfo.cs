using System;
using System.Threading.Tasks;

namespace Coramba.Core.Parallel
{
    public class ParallelRunnerTaskInfo
    {
        public Func<Task> TaskFactory { get; set; }
        public Func<Exception, Task<bool>> OnError { get; set; }
    }
}
