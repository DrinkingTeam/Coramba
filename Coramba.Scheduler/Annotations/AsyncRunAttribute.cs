using System;

namespace Coramba.Scheduler.Annotations
{
    public class AsyncRunAttribute: Attribute
    {
        public bool Enabled { get; }

        public AsyncRunAttribute(bool enabled)
        {
            Enabled = enabled;
        }
    }
}
