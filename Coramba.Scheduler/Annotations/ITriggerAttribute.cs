using System;
using Quartz;

namespace Coramba.Scheduler.Annotations
{
    public interface ITriggerAttribute
    {
        ITrigger CreateTrigger(Type type);
    }
}
