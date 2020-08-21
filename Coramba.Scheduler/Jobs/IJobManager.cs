using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace Coramba.Scheduler.Jobs
{
    public interface IJobManager
    {
        Task<IEnumerable<IJobExecutionContext>> GetCurrentlyExecutingJobs(CancellationToken cancellationToken = default);
        Task TriggerJobAsync(JobDataMap jobDataMap, CancellationToken cancellationToken = default);
        Type JobType { get; }
    }
}
