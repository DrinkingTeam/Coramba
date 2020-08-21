using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coramba.Scheduler.Jobs;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Spi;

namespace Coramba.Scheduler.Services
{
    class SchedulerHostedService : IHostedService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _jobFactory;
        private readonly IEnumerable<IJobCreator> _jobCreators;

        public SchedulerHostedService(
            ISchedulerFactory schedulerFactory,
            IJobFactory jobFactory,
            IEnumerable<IJobCreator> jobCreators)
        {
            _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
            _jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
            _jobCreators = jobCreators ?? throw new ArgumentNullException(nameof(jobCreators));
        }
        protected IScheduler Scheduler { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;

            foreach (var jobCreator in _jobCreators)
            {
                var jobCreationInstruction = await jobCreator.CreateAsync();
                var jobDetail = jobCreationInstruction.JobDetail;
                var triggers = jobCreationInstruction.Triggers?.ToList().AsReadOnly();

                if (jobDetail == null && (triggers == null || triggers.Any()))
                    throw new Exception($"JobDetail or Trigger must be set");

                if (triggers != null && triggers.Any())
                {
                    if (jobDetail != null)
                        await Scheduler.ScheduleJob(jobDetail, triggers, false, cancellationToken);
                    else
                        foreach (var trigger in triggers)
                            await Scheduler.ScheduleJob(trigger, cancellationToken);
                }
                else
                    await Scheduler.AddJob(jobDetail, false, true, cancellationToken);
            }

            await Scheduler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var task = Scheduler?.Shutdown(cancellationToken);
            if (task != null)
                await task;
        }
    }
}
