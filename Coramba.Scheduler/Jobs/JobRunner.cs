using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Humanizer;
using Coramba.Common;
using Coramba.Scheduler.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Coramba.Scheduler.Jobs
{
    public class JobRunner<TJob> : IJob, IDisposable
        where TJob : IJob
    {
        // ReSharper disable once StaticMemberInGenericType
        private static int _runningCount;

        public TJob Job { get; }
        private readonly IServiceScope _scope;
        private bool IsAsyncRunEnabled => typeof(TJob).GetCustomAttribute<AsyncRunAttribute>()?.Enabled ?? true;

        public JobRunner(IServiceScope scope, TJob job)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Job = job ?? throw new ArgumentNullException(nameof(job));
        }

        private void SetParameters(JobDataMap dataMap)
        {
            TypeDescriptor
                .GetProperties(Job)
                .Cast<PropertyDescriptor>()
                .Select(x => new
                {
                    Property = x,
                    Attribute = x.Attributes.OfType<JobParameterAttribute>().FirstOrDefault()
                })
                .Where(x => x.Attribute != null)
                .ForEach(x =>
                {
                    if (dataMap.TryGetValue(x.Attribute.Name ?? x.Property.Name, out var value))
                        x.Property.SetValue(Job, value);
                    else if (x.Attribute.IsRequired)
                        throw new Exception($"Job parameter for property {x.Property.Name} is required");
                });
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var jobType = Job.GetType();
            var logger = (ILogger)_scope.ServiceProvider.GetRequiredService(typeof(ILogger<>).MakeGenericType(jobType));
            using var scope = logger.BeginScope(new Dictionary<string, object>
            {
                {"JobFireId", context.FireInstanceId },
                {"JobGroup", context.JobDetail.Key.Group },
                {"JobName", context.JobDetail.Key.Name },
                {"JobDescription", context.JobDetail.Description }
            });

            if (!IsAsyncRunEnabled && Interlocked.CompareExchange(ref _runningCount, 1, 0) != 0)
            {
                logger.LogTrace($"Job {jobType.FullName} is already started...");
                return;
            }

            try
            {
                SetParameters(context.MergedJobDataMap);

                logger.LogInformation($"Job started...");
                var start = DateTime.UtcNow;

                await Job.Execute(context);

                logger.LogInformation(
                    $"Job finished, elapsed {(DateTime.UtcNow - start).Humanize(culture: CultureInfo.InvariantCulture)}...");
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Job {jobType.FullName} error");
                throw;
            }
            finally
            {
                if (!IsAsyncRunEnabled)
                    Interlocked.Exchange(ref _runningCount, 0);
            }
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
