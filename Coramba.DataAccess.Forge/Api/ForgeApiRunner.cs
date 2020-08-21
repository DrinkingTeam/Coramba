using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Humanizer;
using Coramba.Common;
using Coramba.DataAccess.Forge.Auth;
using Microsoft.Extensions.Logging;

namespace Coramba.DataAccess.Forge.Api
{
    public class ForgeApiRunner<TApi>: IForgeApiRunner<TApi>
    {
        private readonly ILogger Logger;
        protected Configuration Configuration { get; }
        protected IForgeAuth ForgeAuth { get; }

        public ForgeApiRunner(Configuration configuration, IForgeAuth forgeAuth, ILogger<ForgeApiRunner<TApi>> logger)
        {
            Logger = logger;
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            ForgeAuth = forgeAuth;
        }

        protected virtual TApi CreateApi()
        {
            return (TApi)Activator.CreateInstance(typeof(TApi), Configuration);
        }

        private static string GetHeadersLog(IDictionary<string, string> headers)
            => headers
                .Select(x => $"{Environment.NewLine}  {x.Key}={x.Value}")
                .Flatten();

        public async Task<ApiResponse<TResult>> RunAsync<TResult>(Func<TApi, Task<ApiResponse<TResult>>> func, int attempts)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            var api = CreateApi();

            await ForgeAuth.LoginAsync();

            ApiResponse<TResult> response = null;
            for (attempts --; attempts >= 0; attempts --)
            {
                try
                {
                    response = await func(api);
                    break;
                }
                catch (Exception e)
                {
                    if (e is ApiException apiException)
                    {
                        if (apiException.ErrorContent != null)
                            Logger.LogInformation(((object)apiException.ErrorContent)?.ToString());
                        if (apiException.ErrorCode == 429) // too many requests
                        {
                            attempts++;
                            await Task.Delay(TimeSpan.FromSeconds(new Random().Next(1, 20)));
                            continue;
                        }
                    }

                    if (attempts == 0)
                        throw;

                    Logger.LogError(e, $"Error during request");

                    var delay = TimeSpan.FromSeconds(3);
                    Logger.LogInformation($"Retry after {delay.Humanize()}");

                    await Task.Delay(delay);
                }
            }

            Logger.LogInformation($"Status: {response?.StatusCode}, headers:{GetHeadersLog(response?.Headers)}");

            return response;
        }
    }
}
