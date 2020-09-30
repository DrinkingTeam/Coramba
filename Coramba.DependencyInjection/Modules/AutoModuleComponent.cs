using System;
using System.Collections.Generic;
using System.Reflection;

namespace Coramba.DependencyInjection.Modules
{
    public class AutoModuleComponent
    {
        public Assembly[] Assemblies { get; set; }
        public Dictionary<Type, List<Action<IModule>>> Setup { get; } = new Dictionary<Type, List<Action<IModule>>>();
    }
}
