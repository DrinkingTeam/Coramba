using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Autodesk.Forge.Client;
using Humanizer;
using Coramba.Common;
using Coramba.DataAccess.Forge.Auth;
using Microsoft.Extensions.Logging;

namespace Coramba.DataAccess.Forge.Api
{
    class ForgeRawApiRunner : IForgeRawApiRunner
    {
        private const string ForgeApiUrl = "https://developer.api.autodesk.com";

        private readonly Configuration _configuration;
        public IForgeAuth ForgeAuth { get; }
        public ILogger<ForgeRawApiRunner> Logger { get; }

        public ForgeRawApiRunner(IForgeAuth forgeAuth, ILogger<ForgeRawApiRunner> logger,
            Configuration configuration)
        {
            _configuration = configuration;
            ForgeAuth = forgeAuth;
            Logger = logger;
        }

        private static string GetHeadersLog(HttpRequestMessage message, HttpClient client)
            => message
                .Headers
                .If(client?.DefaultRequestHeaders != null, x => x.Union(client.DefaultRequestHeaders))
                .If(message.Content?.Headers != null, x => x.Union(message.Content.Headers))
                .SelectMany(x => x.Value.Select(y => new {x.Key, Value = y}))
                .Select(x => $"{Environment.NewLine}  {x.Key}={x.Value}")
                .Flatten();

        private static string GetHeadersLog(HttpResponseMessage message)
            => message
                .Headers
                .If(message.Content?.Headers != null, x => x.Union(message.Content.Headers))
                .SelectMany(x => x.Value.Select(y => new { x.Key, Value = y }))
                .Select(x => $"{Environment.NewLine}  {x.Key}={x.Value}")
                .Flatten();

        public async Task<HttpResponseMessage> SendAsync(string uri, HttpClient client, int attempts, HttpMethod method, HttpCompletionOption completionOption)
        {
            await ForgeAuth.LoginAsync();

            for (attempts--; attempts >= 0; attempts--)
            {
                try
                {
                    var absoluteUri = new Uri(new Uri(ForgeApiUrl), uri);

                    if (!client.DefaultRequestHeaders.Contains(HttpRequestHeader.Authorization.ToString()))
                        client.DefaultRequestHeaders.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {_configuration.AccessToken}");
                    if (!client.DefaultRequestHeaders.Contains(HttpRequestHeader.ContentType.ToString()))
                        client.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType.ToString(), "application/json");

                    using var request = new HttpRequestMessage(method, absoluteUri);
                    Logger.LogInformation($"{method.ToString().ToUpperInvariant()}: {absoluteUri}");
                    Logger.LogInformation($"Headers:{GetHeadersLog(request, client)}");

                    var response = await client.SendAsync(request, completionOption);

                    if (response.StatusCode == HttpStatusCode.TooManyRequests) // too many requests
                    {
                        attempts++;
                        await Task.Delay(TimeSpan.FromSeconds(new Random().Next(1, 20)));
                        continue;
                    }

                    Logger.LogInformation($"Status: {response.StatusCode}, headers:{GetHeadersLog(response)}");

                    return response;
                }
                catch (Exception e)
                {
                    if (attempts == 0)
                        throw;

                    Logger.LogError(e, $"Error during request");

                    var delay = TimeSpan.FromSeconds(3);
                    Logger.LogInformation($"Retry after {delay.Humanize()}");

                    await Task.Delay(delay);
                }
            }

            throw new Exception($"Unknown error");
        }

        public async Task<Stream> GetStreamAsync(string uri, HttpClient client)
        {
            await ForgeAuth.LoginAsync();

            if (!client.DefaultRequestHeaders.Contains(HttpRequestHeader.Authorization.ToString()))
                client.DefaultRequestHeaders.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {_configuration.AccessToken}");

            var absoluteUri = new Uri(new Uri(ForgeApiUrl), uri);
            Logger.LogInformation($"Forge API request: {absoluteUri}");

            return await client.GetStreamAsync(absoluteUri);
        }
    }
}