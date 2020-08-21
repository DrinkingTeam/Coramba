using System.Linq;
using System.Reflection;
using Coramba.Common;
using Coramba.DependencyInjection;
using Coramba.Scheduler.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Coramba.Scheduler
{
    public static class RegistrationExtensions
    {
        public static RegistrationModule SchedulerModule(this IServiceCollection services)
            => services.Module<RegistrationModule>();

        public static void AddSchedulerModule(this IServiceCollection services)
            => services.AddModule<RegistrationModule>();

        public static void JobModule<TJob>(this IServiceCollection services)
            where TJob : class, IJob
            => services.Module<JobModule<TJob>>();

        public static void AddJobsFromAssembly(this IServiceCollection services, Assembly assembly)
        {
            assembly
                .GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && t.IsPublic)
                .Where(t => typeof(IJob).IsAssignableFrom(t))
                .ForEach(t =>
                {
                    services.AddModule(typeof(JobModule<>).MakeGenericType(t));
                });
        }
    }
}
