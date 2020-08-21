using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Coramba.DependencyInjection;
using Coramba.DependencyInjection.ScopedServices;

namespace Coramba.Core.Parallel
{
    public static class ParallelRunnerBuilderExtensions
    {
        public static ParallelRunnerBuilder Task<TService>(
            [NotNull] this ParallelRunnerBuilder builder,
            [NotNull] IScopedRunner scopedRunner,
            [NotNull] Func<TService, Task> taskFactory,
            Func<TService, Exception, Task<bool>> onError = null
            )
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (scopedRunner == null) throw new ArgumentNullException(nameof(scopedRunner));
            if (taskFactory == null) throw new ArgumentNullException(nameof(taskFactory));

            Func<Exception, Task<bool>> onTaskError = null;
            if (onError != null)
                onTaskError = e => scopedRunner.RunAsync<TService, bool>(s => onError(s, e));

            return builder.Task(() => scopedRunner.RunAsync(taskFactory), onTaskError);
        }

        public static ParallelRunnerBuilder Task<TService>(
            [NotNull] this ParallelRunnerBuilder builder,
            [NotNull] IScopedRunner scopedRunner,
            [NotNull] Func<TService, Task> taskFactory,
            Func<TService, Exception, Task> onError = null
        )
            => builder.Task(scopedRunner, taskFactory, async (s, e) =>
            {
                await onError(s, e);
                return true;
            });

        public static ParallelRunnerBuilder Tasks<TService, T>(
            [NotNull] this ParallelRunnerBuilder builder,
            [NotNull] IEnumerable<T> items,
            [NotNull] IScopedRunner scopedRunner,
            [NotNull] Func<TService, T, Task> taskFactory,
            Func<TService, T, Exception, Task<bool>> onError = null
        )
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (items == null) throw new ArgumentNullException(nameof(items));
            if (scopedRunner == null) throw new ArgumentNullException(nameof(scopedRunner));
            if (taskFactory == null) throw new ArgumentNullException(nameof(taskFactory));

            Func<T, Exception, Task<bool>> onTaskError = null;
            if (onError != null)
                onTaskError = (t, e) => scopedRunner.RunAsync<TService, bool>(s => onError(s, t, e));

            return builder.Tasks(items,
                t => scopedRunner.RunAsync<TService>(s => taskFactory(s, t)),
                onTaskError);
        }

        public static ParallelRunnerBuilder Tasks<TService, T>(
            [NotNull] this ParallelRunnerBuilder builder,
            [NotNull] IEnumerable<T> items,
            [NotNull] IScopedRunner scopedRunner,
            [NotNull] Func<TService, T, Task> taskFactory,
            Func<TService, T, Exception, Task> onError = null
        )
            => builder.Tasks(items, scopedRunner, taskFactory, async (s, x, e) =>
            {
                await onError(s, x, e);
                return true;
            });
    }
}
