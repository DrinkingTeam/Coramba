using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Coramba.DataAccess.Forge.Api
{
    public interface IForgeRawApiRunner
    {
        Task<HttpResponseMessage> SendAsync(string uri, HttpClient client, int attempts, HttpMethod method, HttpCompletionOption completionOption);
        Task<Stream> GetStreamAsync(string uri, HttpClient client);
    }
}