using System;
using Quartz;

namespace Coramba.Scheduler.Jobs
{
    public interface IJobKeyGetter
    {
        JobKey GetKey(Type type);
    }
}
