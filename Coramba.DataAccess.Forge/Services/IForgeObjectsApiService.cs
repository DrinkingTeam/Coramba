using System.IO;
using System.Threading.Tasks;

namespace Coramba.DataAccess.Forge.Services
{
    public interface IForgeObjectsApiService
    {
        public Task<string> UploadAsync(string bucketKey, string objectName, long contentLength, Stream stream);
    }
}
