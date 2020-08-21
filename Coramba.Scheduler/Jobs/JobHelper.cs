using Quartz;

namespace Coramba.Scheduler.Jobs
{
    static class JobHelper
    {
        public static string FormatJobName(JobKey jobKey)
            => jobKey.Group == null ? jobKey.Name : $"{jobKey.Group}.{jobKey.Name}";
    }
}
