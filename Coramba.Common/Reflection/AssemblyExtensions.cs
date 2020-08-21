using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Coramba.Common.Reflection
{
    public static class AssemblyExtensions
    {
        private static void GetLoadedReferencedAssemblies(Assembly root, Func<AssemblyName, bool> filter, HashSet<Assembly> assemblies)
        {
            assemblies.Add(root);

            var q = root.GetReferencedAssemblies().OrderBy(x => x.Name).ToList();

            root
                .GetReferencedAssemblies()
                .If(filter != null, x => x.Where(filter))
                .Select(Assembly.Load)
                .Where(a => !assemblies.Contains(a))
                .ForEach(a =>
                {
                    GetLoadedReferencedAssemblies(a, filter, assemblies);
                });
        }

        public static HashSet<Assembly> GetLoadedReferencedAssemblies(this Assembly root, Func<AssemblyName, bool> filter = null)
        {
            var result = new HashSet<Assembly>();
            GetLoadedReferencedAssemblies(root, filter, result);
            return result;
        }
    }
}
