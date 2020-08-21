using System;
using Quartz;

namespace Coramba.Scheduler.Annotations
{
    public class SimpleSchedulerAttribute : Attribute, ITriggerAttribute
    {
        public TimeSpan Interval { get; }

        public SimpleSchedulerAttribute(int seconds = 0, int minutes = 0, int hours = 0, int days = 0)
        {
            Interval = new TimeSpan(days, hours, minutes, seconds);
        }

        public ITrigger CreateTrigger(Type type)
        {
            return TriggerBuilder
                .Create()
                .WithSimpleSchedule(s => s
                        .WithInterval(Interval)
                        .RepeatForever())
                .Build();
        }
    }
}
