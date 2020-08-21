using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Coramba.Common;
using Quartz;

namespace Coramba.Scheduler.Jobs
{
    class JobKeyGetter : IJobKeyGetter
    {
        const string JobNameIgnoreSuffix = "Job";

        public JobKey GetKey([NotNull] Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var name = type.GetCustomAttribute<DisplayAttribute>()?.GetName()
                       ?? type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;

            return name != null ? new JobKey(name) : new JobKey(type.Name.TrimEnd(JobNameIgnoreSuffix));
        }
    }
}