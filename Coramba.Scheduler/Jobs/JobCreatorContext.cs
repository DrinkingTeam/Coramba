using Quartz;

namespace Coramba.Scheduler.Jobs
{
    public class JobCreatorContext<TJob>
    {
        public JobKey JobKey { get; set; }
        public string JobDescription { get; set; }
    }
}
