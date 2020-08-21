using System;
using System.Net;
using System.Threading.Tasks;
using Autodesk.Forge.Client;

namespace Coramba.DataAccess.Forge.Api
{
    public static class ForgeApiRunnerExtensions
    {
        private const int GetAttempts = 3;
        private const int PostAttempts = 1;

        public static async Task<ApiResponse<TResult>> GetAsync<TApi, TResult>(
            this IForgeApiRunner<TApi> apiRunner,
            Func<TApi, Task<ApiResponse<TResult>>> func,
            params HttpStatusCode[] statusCodes)
        {
            var response = await apiRunner.RunAsync(func, GetAttempts);
            ForgeApiValidators.StatusCodes(response, statusCodes);
            return response;
        }

        public static async Task<TResult> GetOkAsync<TApi, TResult>(
            this IForgeApiRunner<TApi> apiRunner,
            Func<TApi, Task<ApiResponse<TResult>>> func)
            => (await apiRunner.GetAsync(func, HttpStatusCode.OK)).Data;

        public static Task<ApiResponse<TResult>> GetOkOrAcceptedAsync<TApi, TResult>(
            this IForgeApiRunner<TApi> apiRunner,
            Func<TApi, Task<ApiResponse<TResult>>> func)
            => apiRunner.GetAsync(func, HttpStatusCode.OK, HttpStatusCode.Accepted);

        public static async Task<ApiResponse<TResult>> PostAsync<TApi, TResult>(
            this IForgeApiRunner<TApi> apiRunner,
            Func<TApi, Task<ApiResponse<TResult>>> func,
            int attempts = PostAttempts,
            params HttpStatusCode[] statusCodes)
        {
            var response = await apiRunner.RunAsync(func, attempts);
            ForgeApiValidators.StatusCodes(response, statusCodes);
            return response;
        }

        public static async Task<TResult> PostOkAsync<TApi, TResult>(
            this IForgeApiRunner<TApi> apiRunner,
            Func<TApi, Task<ApiResponse<TResult>>> func)
            => (await apiRunner.PostAsync(func, PostAttempts, HttpStatusCode.OK)).Data;

        public static async Task<ApiResponse<TResult>> PostOkOrAcceptedAsync<TApi, TResult>(
            this IForgeApiRunner<TApi> apiRunner,
            Func<TApi, Task<ApiResponse<TResult>>> func,
            int attempts = PostAttempts)
            => (await apiRunner.PostAsync(func, attempts, HttpStatusCode.OK, HttpStatusCode.Accepted));

        //public static async Task<IEnumerable<TResult>> SelectPagedAsync<TApi, TResult>(
        //    this IForgeApiRunner<TApi> apiRunner,
        //    Func<TApi, int, int, Task<ApiResponse<IEnumerable<TResult>>>> selector,
        //    int pageSize,
        //    Action<ApiResponse<IEnumerable<TResult>>> validator)
        //{
        //    if (apiRunner == null) throw new ArgumentNullException(nameof(apiRunner));
        //    if (selector == null) throw new ArgumentNullException(nameof(selector));

        //    var result = new List<TResult>();
        //    for (var i = 0; ; i += pageSize)
        //    {
        //        var list = (await apiRunner.RunAsync(api => selector(api, i, pageSize), validator))?.ToList();
        //        if (list == null || !list.Any())
        //            break;

        //        result.AddRange(list);
        //    }

        //    return result;
        //}
    }
}
