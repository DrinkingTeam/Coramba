using System;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace Coramba.Scheduler.Jobs
{
    class JobFactory : IJobFactory
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        
        public JobFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobType = bundle.JobDetail.JobType;
            var scope = _serviceScopeFactory.CreateScope();
            var jobRunnerType = typeof(JobRunner<>).MakeGenericType(jobType);

            return (IJob)ActivatorUtilities.CreateInstance(scope.ServiceProvider, jobRunnerType, scope);
        }

        public void ReturnJob(IJob job)
        {
            (job as IDisposable)?.Dispose();
        }
    }
}
