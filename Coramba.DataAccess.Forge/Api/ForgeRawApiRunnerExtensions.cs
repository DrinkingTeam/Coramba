using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Coramba.DataAccess.Forge.Api
{
    public static class ForgeRawApiRunnerExtensions
    {
        private const int AttemptsForGet = 3;
        private const int AttemptsForHead = 3;

        private static async Task<HttpResponseMessage> SendAsync(this IForgeRawApiRunner apiRunner,
            string uri,
            HttpClient client,
            int attempts,
            HttpMethod method,
            HttpCompletionOption completionOption,
            params HttpStatusCode[] statusCodes)
        {
            var response = await apiRunner.SendAsync(uri, client, attempts, method, completionOption);
            ForgeApiValidators.StatusCodes(response, statusCodes);
            return response;
        }

        public static Task<HttpResponseMessage> GetAsync(this IForgeRawApiRunner apiRunner,
            string uri,
            HttpClient client,
            params HttpStatusCode[] statusCodes)
            => apiRunner.SendAsync(uri, client, AttemptsForGet, HttpMethod.Get, HttpCompletionOption.ResponseHeadersRead, statusCodes);

        public static Task<HttpResponseMessage> HeadAsync(this IForgeRawApiRunner apiRunner,
            string uri,
            HttpClient client,
            params HttpStatusCode[] statusCodes)
            => apiRunner.SendAsync(uri, client, AttemptsForHead, HttpMethod.Head, HttpCompletionOption.ResponseHeadersRead, statusCodes);

        public static Task<HttpResponseMessage> GetOkAsync(this IForgeRawApiRunner apiRunner,
            string uri, HttpClient client)
            => apiRunner.GetAsync(uri, client, HttpStatusCode.OK);

        public static Task<HttpResponseMessage> GetOkOrAcceptedAsync(this IForgeRawApiRunner apiRunner,
            string uri, HttpClient client)
            => apiRunner.GetAsync(uri, client, HttpStatusCode.OK, HttpStatusCode.Accepted);

        public static Task<HttpResponseMessage> HeadOkOrAcceptedAsync(this IForgeRawApiRunner apiRunner,
            string uri, HttpClient client)
            => apiRunner.HeadAsync(uri, client, HttpStatusCode.OK, HttpStatusCode.Accepted);

        public static Task<HttpResponseMessage> GetPartialContentAsync(this IForgeRawApiRunner apiRunner,
            string uri, HttpClient client)
            => apiRunner.GetAsync(uri, client, HttpStatusCode.PartialContent);
    }
}
