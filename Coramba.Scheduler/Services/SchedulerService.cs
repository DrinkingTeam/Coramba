using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Quartz;

namespace Coramba.Scheduler.Services
{
    class SchedulerService : ISchedulerService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1);

        public SchedulerService(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
        }

        public async Task<IEnumerable<IJobExecutionContext>> GetCurrentlyExecutingJobs(CancellationToken cancellationToken = default)
        {
            await _mutex.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                return await (await _schedulerFactory.GetScheduler(cancellationToken))
                    .GetCurrentlyExecutingJobs(cancellationToken);
            }
            finally
            {
                _mutex.Release();
            }
        }

        public async Task TriggerJobAsync(JobKey jobKey, JobDataMap jobDataMap, CancellationToken cancellationToken = default)
        {
            await _mutex.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await (await _schedulerFactory.GetScheduler(cancellationToken))
                    .TriggerJob(jobKey, jobDataMap, cancellationToken);
            }
            finally
            {
                _mutex.Release();
            }
        }
    }
}
