using Quartz;

namespace Coramba.Scheduler.Jobs
{
    public interface IJobManager<TJob> : IJobManager
        where TJob : IJob
    {
    }
}
