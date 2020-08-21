using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coramba.Common;
using Coramba.Scheduler.Services;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Coramba.Scheduler.Jobs
{
    public class JobManager<TJob> : IJobManager<TJob>
        where TJob : IJob
    {
        private readonly IJobKeyGetter _jobKeyGetter;
        private readonly ISchedulerService _schedulerService;
        private readonly ILogger<JobManager<TJob>> _logger;

        public Type JobType => typeof(TJob);

        public JobManager(IJobKeyGetter jobKeyGetter, ISchedulerService schedulerService, ILogger<JobManager<TJob>> logger)
        {
            _jobKeyGetter = jobKeyGetter ?? throw new ArgumentNullException(nameof(jobKeyGetter));
            _schedulerService = schedulerService ?? throw new ArgumentNullException(nameof(schedulerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<IJobExecutionContext>> GetCurrentlyExecutingJobs(CancellationToken cancellationToken = default)
        {
            var jobContexts = await _schedulerService.GetCurrentlyExecutingJobs(cancellationToken);
            return jobContexts.Where(x => x.JobDetail.JobType == typeof(TJob));
        }

        public async Task TriggerJobAsync(JobDataMap jobDataMap, CancellationToken cancellationToken = default)
        {
            var nonEmptyjobDataMap = jobDataMap ?? new JobDataMap();
            _logger.LogInformation($"Job trigged: {typeof(TJob)}{nonEmptyjobDataMap.Select(x => $"{Environment.NewLine}{x.Key} = {x.Value}").Flatten()}");

            var jobKey = _jobKeyGetter.GetKey(typeof(TJob));
            await _schedulerService.TriggerJobAsync(jobKey, jobDataMap, cancellationToken);
        }
    }
}