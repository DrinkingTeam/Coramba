using System.Collections.Generic;
using Quartz;

namespace Coramba.Scheduler.Jobs
{
    public class JobCreationInstruction
    {
        public IJobDetail JobDetail { get; set; }
        public IEnumerable<ITrigger> Triggers { get; set; }
    }
}
