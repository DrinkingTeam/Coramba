using Coramba.DependencyInjection;
using Coramba.DependencyInjection.Modules;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Coramba.Scheduler.Jobs
{
    public class JobModule<TJob> : ModuleBuilder<JobModule<TJob>, JobModule<TJob>.ComponentInfo>
        where TJob : class, IJob
    {
        public class ComponentInfo
        {
            public string SchedulerName { get; set; }
            public JobKey JobKey { get; set; }
            public string JobDescription { get; set; }
        }

        public JobModule<TJob> SchedulerName(string name)
            => WithComponent(c => c.SchedulerName = name);

        public JobModule<TJob> JobKey(string name, string group = null)
            => Key(new JobKey(name, group));

        public JobModule<TJob> Key(JobKey key)
            => WithComponent(c => c.JobKey = key);

        public JobModule<TJob> JobDescription(string description)
            => WithComponent(c => c.JobDescription = description);

        protected override void Register()
        {
            Services.AddSingleton<IJobCreator>(sp =>
                ActivatorUtilities.CreateInstance<JobCreator<TJob>>(sp, new JobCreatorContext<TJob>
                {
                    JobKey = Component.JobKey,
                    JobDescription =  Component.JobDescription
                }));

            Services.AddMultiple(
                new [] { typeof(IJobManager<TJob>), typeof(IJobManager) },
                typeof(JobManager<TJob>),
                ServiceLifetime.Singleton);

            Services.AddScoped<TJob>();
        }
    }
}
