using System;
using System.Collections.Generic;
using System.Reflection;

namespace Coramba.DependencyInjection.Modules
{
    public class AutoModuleBuilder
    {
        internal AutoModuleComponent Component { get; } = new AutoModuleComponent();

        public AutoModuleBuilder Assemblies(params Assembly[] assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            if (assemblies.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(assemblies));

            Component.Assemblies = assemblies;

            return this;
        }

        public AutoModuleBuilder Setup<TModule>(Action<TModule> moduleSetup)
            where TModule: IModule
        {
            Component.Setup.TryAdd(typeof(TModule), new List<Action<IModule>>());
            Component.Setup[typeof(TModule)].Add(m => moduleSetup((TModule)m));

            return this;
        }
    }
}
