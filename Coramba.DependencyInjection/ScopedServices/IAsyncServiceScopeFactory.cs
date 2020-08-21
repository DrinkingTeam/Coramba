using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Coramba.DependencyInjection.ScopedServices
{
    public interface IAsyncServiceScopeFactory
    {
        Task<IServiceScope> CreateScopeAsync();
    }
}
