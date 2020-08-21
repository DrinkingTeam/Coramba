using System;

namespace Coramba.Scheduler.Jobs
{
    public interface IJobDescriptionGetter
    {
        string GetDescription(Type type);
    }
}
