using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DependencyInjection.ScopedServices
{
    public class ScopedRunner: IScopedRunner
    {
        private readonly IAsyncServiceScopeFactory _asyncServiceScopeFactory;

        public ScopedRunner(IAsyncServiceScopeFactory asyncServiceScopeFactory)
        {
            _asyncServiceScopeFactory = asyncServiceScopeFactory ?? throw new ArgumentNullException(nameof(asyncServiceScopeFactory));
        }

        public async Task<TResult> RunAsync<TResult>(Type[] serviceTypes, Func<object[], Task<TResult>> runner, Func<IServiceScope, Task> setupScope)
        {
            if (serviceTypes == null) throw new ArgumentNullException(nameof(serviceTypes));
            using var scope = await _asyncServiceScopeFactory.CreateScopeAsync();

            if (setupScope != null)
                await setupScope(scope);

            var services = new List<object>();
            foreach (var type in serviceTypes)
            {
                services.Add(scope.ServiceProvider.GetRequiredService(type));
            }

            return await runner(services.ToArray());
        }
    }
}
