using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace Coramba.Scheduler.Services
{
    public interface ISchedulerService
    {
        Task<IEnumerable<IJobExecutionContext>> GetCurrentlyExecutingJobs(CancellationToken cancellationToken = default);

        Task TriggerJobAsync(JobKey jobKey, JobDataMap jobDataMap, CancellationToken cancellationToken = default);
    }
}
