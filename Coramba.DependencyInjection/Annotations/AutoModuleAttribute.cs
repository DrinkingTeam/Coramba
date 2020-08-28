using System;

namespace Coramba.DependencyInjection.Annotations
{
    /// <summary>
    /// Module will automatically register in DI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoModuleAttribute: Attribute
    {
        public Type[] DependsOn;
        public Type[] IncludeAssemblyOfTypes;
    }
}
