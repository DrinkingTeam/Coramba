using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coramba.Scheduler.Annotations;
using Quartz;

namespace Coramba.Scheduler.Jobs
{
    public class JobCreator<TJob> : IJobCreator
    {
        private JobKey JobKey { get; }
        public string JobDescription { get; }
        private readonly IJobKeyGetter _jobKeyGetter;
        private readonly IJobDescriptionGetter _jobDescriptionGetter;

        public JobCreator(
            JobCreatorContext<TJob> context,
            IJobKeyGetter jobKeyGetter,
            IJobDescriptionGetter jobDescriptionGetter)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            JobKey = context.JobKey;
            JobDescription = context.JobDescription;
            _jobKeyGetter = jobKeyGetter ?? throw new ArgumentNullException(nameof(jobKeyGetter));
            _jobDescriptionGetter = jobDescriptionGetter ?? throw new ArgumentNullException(nameof(jobDescriptionGetter));
        }

        private ITrigger CreateTrigger(ITriggerAttribute triggerAttribute)
        {
            return triggerAttribute.CreateTrigger(typeof(TJob));
        }

        public Task<JobCreationInstruction> CreateAsync()
        {
            var jobType = typeof(TJob);
            var jobKey = JobKey ?? _jobKeyGetter.GetKey(jobType);
            var jobDescription = JobDescription ?? _jobDescriptionGetter.GetDescription(jobType);

            var jobDetail = JobBuilder
                .Create(jobType)
                .WithIdentity(jobKey)
                .WithDescription(jobDescription)
                .StoreDurably()
                .Build();

            var triggers =
                ((IEnumerable<ITriggerAttribute>) jobType.GetCustomAttributes(typeof(ITriggerAttribute), true))
                .Select(CreateTrigger)
                .ToList();

            return Task.FromResult(new JobCreationInstruction
            {
                JobDetail = jobDetail,
                Triggers = triggers
            });
        }
    }
}
