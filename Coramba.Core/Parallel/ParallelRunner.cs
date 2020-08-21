using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Coramba.Core.Parallel
{
    public class ParallelRunner: IParallelRunner
    {
        private readonly ILogger<ParallelRunner> _logger;

        public ParallelRunner(ILogger<ParallelRunner> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        IDisposable BeginLoggerScope(string taskId)
            => _logger.BeginScope(new {TaskId = taskId});

        protected virtual async Task CreateTask(ParallelRunnerTaskInfo taskInfo, string taskId)
        {
            using (BeginLoggerScope(taskId))
            {
                _logger.LogInformation($"Starting task {taskId}...");
                try
                {
                    await taskInfo.TaskFactory();

                    _logger.LogInformation($"Task {taskId} is finished successfully");
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Task {taskId} is fail");

                    bool shouldThrow;
                    try
                    {
                        shouldThrow = taskInfo.OnError == null || !await taskInfo.OnError(exception);
                    }
                    catch (Exception innerException)
                    {
                        _logger.LogError(innerException, $"Task {taskId} is fail while error processing");
                        throw new AggregateException(exception, innerException);
                    }

                    if (shouldThrow)
                        throw;
                }
            }
        }

        protected virtual void OnCancel(ParallelRunnerTaskInfo taskInfo, string taskId)
        {
            _logger.LogInformation($"Task {taskId} is canceled (post processing)");
        }

        protected virtual void OnError(ParallelRunnerTaskInfo taskInfo, Exception exception, string taskId)
        {
            _logger.LogError(exception, $"Task {taskId} is fail (post processing)");
        }

        public async Task RunAsync(ParallelRunnerParameters parameters)
        {
            using var throttler = new SemaphoreSlim(parameters.MaxConcurrency);
            var tasks = new List<Task>();
            
            _logger.LogInformation($"Starting tasks...");

            var errors = new ConcurrentBag<Exception>();
            foreach (var taskInfo in parameters.Tasks)
            {
                await throttler.WaitAsync(parameters.CancellationToken);

                var taskId = Guid.NewGuid().ToString("N");
                var task = CreateTask(taskInfo, taskId)
                    .ContinueWith(t =>
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        throttler.Release();

                        switch (t.Status)
                        {
                            case TaskStatus.Canceled:
                                OnCancel(taskInfo, taskId);
                                break;
                            case TaskStatus.Faulted:
                                OnError(taskInfo, t.Exception, taskId);
                                errors.Add(t.Exception);
                                break;
                            case TaskStatus.RanToCompletion:
                                break;
                            default:
                                _logger.LogWarning($"Task {taskId}: unknown status {t.Status}.");
                                break;
                        }
                    }, parameters.CancellationToken);

                if (errors.Count > 0)
                    break;

                parameters.CancellationToken.ThrowIfCancellationRequested();

                tasks.Add(task);
            }

            _logger.LogInformation($"Waiting tasks for completion...");

            await Task.WhenAll(tasks);

            if (errors.Any())
            {
                _logger.LogInformation($"One or more errors occurred.");
                throw new AggregateException(errors);
            }

            _logger.LogInformation($"All tasks are finished.");
        }
    }
}
