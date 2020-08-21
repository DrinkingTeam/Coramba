using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Quartz;

namespace Coramba.Scheduler.Jobs
{
    class JobDescriptionGetter : IJobDescriptionGetter
    {
        private readonly IJobKeyGetter _jobKeyGetter;

        public JobDescriptionGetter(IJobKeyGetter jobKeyGetter)
        {
            _jobKeyGetter = jobKeyGetter ?? throw new ArgumentNullException(nameof(jobKeyGetter));
        }

        private string GetDescriptionByJobKey(JobKey jobKey)
            => $"{jobKey.Group}.{jobKey.Name}";

        public string GetDescription(Type type)
        {
            return
                type.GetCustomAttribute<DisplayAttribute>()?.GetDescription()
                ?? GetDescriptionByJobKey(_jobKeyGetter.GetKey(type));
        }
    }
}