using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Coramba.Common;
using Coramba.DependencyInjection.Annotations;

namespace Coramba.DependencyInjection.Modules
{
    public static class AutoModuleWalker
    {
        private static IEnumerable<Type> GetAssemblyModules(Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(t => t.GetCustomAttribute<AutoModuleAttribute>() != null)
                .Where(t => !t.IsAbstract);
        }

        public static IEnumerable<Type> GetModules(HashSet<Assembly> assemblies)
        {
            var types = assemblies
                .SelectMany(GetAssemblyModules);

            types = Dfs<Type>.Reachable(types, t =>
            {
                if (!typeof(IModule).IsAssignableFrom(t))
                    throw new Exception($"Auto module must implements {typeof(IModule)}");
                var attr = t.GetCustomAttribute<AutoModuleAttribute>();

                var result = new List<Type>();
                if (attr.DependsOn != null)
                    result.AddRange(attr.DependsOn);

                if (attr.IncludeAssemblyOfTypes != null)
                    result.AddRange(attr
                        .IncludeAssemblyOfTypes
                        .SelectMany(x => GetAssemblyModules(x.Assembly)));

                return result;
            });

            return Dfs<Type>.TopoSort(types, t =>
            {
                if (!typeof(IModule).IsAssignableFrom(t))
                    throw new Exception($"Auto module must implements {typeof(IModule)}");

                return t.GetCustomAttribute<AutoModuleAttribute>().DependsOn;
            });
        }
    }
}
