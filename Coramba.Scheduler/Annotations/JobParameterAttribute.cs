using System;

namespace Coramba.Scheduler.Annotations
{
    public class JobParameterAttribute : Attribute
    {
        public bool IsRequired { get; set; } = true;
        public string Name { get; }

        public JobParameterAttribute(string name = null)
        {
            Name = name;
        }
    }
}
