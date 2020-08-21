using System;
using System.Threading.Tasks;
using Autodesk.Forge.Client;

namespace Coramba.DataAccess.Forge.Api
{
    public interface IForgeApiRunner<TApi>
    {
        Task<ApiResponse<TResult>> RunAsync<TResult>(Func<TApi, Task<ApiResponse<TResult>>> func, int attempts);
    }
}