using System.Collections.Generic;
using System.Reflection;
using Coramba.DependencyInjection.Annotations;
using Coramba.DependencyInjection.Modules;
using Coramba.Scheduler.Jobs;
using Coramba.Scheduler.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Coramba.Scheduler
{
    [AutoModule(DependsOn = new []
    {
        typeof(DependencyInjection.RegistrationModule)
    })]
    public class RegistrationModule : ModuleBuilder<RegistrationModule, RegistrationModule.ComponentInfo>
    {
        public class ComponentInfo
        {
            public List<Assembly> Assemblies { get; } = new List<Assembly>();
        }

        public RegistrationModule FromAssembly(Assembly assembly)
            => WithComponent(c => c.Assemblies.Add(assembly));

        protected override void Register()
        {
            Component
                .Assemblies
                .ForEach(Services.AddJobsFromAssembly);
        }

        protected override void RegisterOnce()
        {
            Services.AddHostedService<SchedulerHostedService>();
            Services.TryAddSingleton<ISchedulerService, SchedulerService>();
            Services.TryAddSingleton<IJobFactory, JobFactory>();
            Services.TryAddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            Services.TryAddSingleton<IJobKeyGetter, JobKeyGetter>();
            Services.TryAddSingleton<IJobDescriptionGetter, JobDescriptionGetter>();
        }
    }
}
