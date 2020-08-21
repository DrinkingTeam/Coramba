using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DependencyInjection.ScopedServices
{
    class AsyncServiceScopeFactory : IAsyncServiceScopeFactory
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public AsyncServiceScopeFactory(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public Task<IServiceScope> CreateScopeAsync() => Task.FromResult(_serviceScopeFactory.CreateScope());
    }
}