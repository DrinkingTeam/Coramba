using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DependencyInjection.ScopedServices
{
    public interface IScopedRunner
    {
        Task<TResult> RunAsync<TResult>(Type[] serviceTypes, Func<object[], Task<TResult>> runner, Func<IServiceScope, Task> setupScope = null);
    }
}
