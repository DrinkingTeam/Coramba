using System;
using System.Threading.Tasks;

namespace Coramba.DependencyInjection.ScopedServices
{
    public static class ScopedRunnerExtensions
    {
        public static Task<TResult> RunAsync<TService, TResult>(this IScopedRunner scopedRunner, Func<TService, Task<TResult>> runner)
            => scopedRunner.RunAsync(new [] {typeof(TService)},
                services =>  runner((TService)services[0]));

        public static Task<TResult> RunAsync<TService1, TService2, TResult>(this IScopedRunner scopedRunner, Func<TService1, TService2, Task<TResult>> runner)
            => scopedRunner.RunAsync(new[] { typeof(TService1), typeof(TService2) },
                services => runner((TService1)services[0], (TService2)services[0]));

        public static Task<TResult> RunAsync<TService1, TService2, TService3, TResult>(this IScopedRunner scopedRunner, Func<TService1, TService2, TService3, Task<TResult>> runner)
            => scopedRunner.RunAsync(new[] { typeof(TService1), typeof(TService2), typeof(TService3) },
                services => runner((TService1)services[0], (TService2)services[1], (TService3)services[2]));

        public static Task<TResult> RunAsync<TService1, TService2, TService3, TService4, TResult>(this IScopedRunner scopedRunner, Func<TService1, TService2, TService3, TService4, Task<TResult>> runner)
            => scopedRunner.RunAsync(new[] { typeof(TService1), typeof(TService2), typeof(TService3), typeof(TService4) },
                services => runner((TService1)services[0], (TService2)services[1], (TService3)services[2], (TService4)services[3]));

        public static Task<TResult> RunAsync<TService1, TService2, TService3, TService4, TService5, TResult>(this IScopedRunner scopedRunner, Func<TService1, TService2, TService3, TService4, TService5, Task<TResult>> runner)
            => scopedRunner.RunAsync(new[] { typeof(TService1), typeof(TService2), typeof(TService3), typeof(TService4), typeof(TService5) },
                services => runner((TService1)services[0], (TService2)services[1], (TService3)services[2], (TService4)services[3], (TService5)services[4]));

        public static async Task RunAsync<TService>(this IScopedRunner scopedRunner, Func<TService, Task> runner)
            => await scopedRunner.RunAsync<TService, object>(async s =>
            {
                await runner(s);
                return null;
            });

        public static async Task RunAsync<TService1, TService2>(this IScopedRunner scopedRunner, Func<TService1, TService2, Task> runner)
            => await scopedRunner.RunAsync<TService1, TService2, object>(async (s1, s2) =>
            {
                await runner(s1, s2);
                return null;
            });

        public static async Task RunAsync<TService1, TService2, TService3>(this IScopedRunner scopedRunner, Func<TService1, TService2, TService3, Task> runner)
            => await scopedRunner.RunAsync<TService1, TService2, TService3, object>(async (s1, s2, s3) =>
            {
                await runner(s1, s2, s3);
                return null;
            });

        public static async Task RunAsync<TService1, TService2, TService3, TService4>(this IScopedRunner scopedRunner, Func<TService1, TService2, TService3, TService4, Task> runner)
            => await scopedRunner.RunAsync<TService1, TService2, TService3, TService4, object>(async (s1, s2, s3, s4) =>
            {
                await runner(s1, s2, s3, s4);
                return null;
            });

        public static async Task RunAsync<TService1, TService2, TService3, TService4, TService5>(this IScopedRunner scopedRunner, Func<TService1, TService2, TService3, TService4, TService5, Task> runner)
            => await scopedRunner.RunAsync<TService1, TService2, TService3, TService4, TService5, object>(async (s1, s2, s3, s4, s5) =>
            {
                await runner(s1, s2, s3, s4, s5);
                return null;
            });

        public static Task<TResult> RunAsync<TService1, TResult>(this IScopedRunner scopedRunner, Type serviceType1, Func<TService1, Task<TResult>> runner)
            => scopedRunner.RunAsync(new[] { serviceType1 },
                services => runner((TService1)services[0]));
    }
}
