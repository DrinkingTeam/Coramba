using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coramba.Common;

namespace Coramba.Core.Parallel
{
    public class ParallelRunnerBuilder: Builder<ParallelRunnerBuilder, ParallelRunnerParameters>
    {
        protected ParallelRunnerBuilder()
        {
        }

        public static ParallelRunnerBuilder Create()
            => new ParallelRunnerBuilder();

        public ParallelRunnerBuilder MaxConcurrency(int value)
            => WithComponent(c => c.MaxConcurrency = value);

        public ParallelRunnerBuilder Task(Func<Task> taskFactory, Func<Exception, Task<bool>> onError = null)
        {
            if (taskFactory == null) throw new ArgumentNullException(nameof(taskFactory));
            if (onError == null) throw new ArgumentNullException(nameof(onError));
            return WithComponent(c => c.Tasks.Add(new ParallelRunnerTaskInfo
            {
                TaskFactory = taskFactory,
                OnError = onError
            }));
        }

        public ParallelRunnerBuilder Task(Func<Task> taskFactory, Func<Exception, Task> onError = null)
            => Task(taskFactory, async e =>
            {
                await onError(e);
                return true;
            });

        public ParallelRunnerBuilder Tasks<T>(IEnumerable<T> items, Func<T, Task> taskFactory, Func<T, Exception, Task<bool>> onError = null)
            => WithComponent(c => c.Tasks.AddRange(items.Select(x => new ParallelRunnerTaskInfo
            {
                TaskFactory = () => taskFactory(x),
                OnError = onError == null ? null : (Func<Exception, Task<bool>>)(e => onError(x, e))
            })));

        public ParallelRunnerBuilder Tasks<T>(IEnumerable<T> items, Func<T, Task> taskFactory,
            Func<T, Exception, Task> onError)
            => Tasks(items, taskFactory, async (x, e) =>
            {
                await onError(x, e);
                return true;
            });

        public ParallelRunnerParameters Build()
            => Component;

        public Task RunAsync(IParallelRunner parallelRunner)
        {
            if (parallelRunner == null) throw new ArgumentNullException(nameof(parallelRunner));
            return parallelRunner.RunAsync(Build());
        }
    }
}
