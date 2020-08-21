using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Autodesk.Forge;
using Coramba.DataAccess.Forge.Api;

namespace Coramba.DataAccess.Forge.Services
{
    class ForgeObjectsApiService : IForgeObjectsApiService
    {
        private readonly IForgeApiRunner<ObjectsApi> _objectApiRunner;

        public ForgeObjectsApiService(IForgeApiRunner<ObjectsApi> objectApiRunner)
        {
            _objectApiRunner = objectApiRunner;
        }

        public async Task<string> UploadAsync(string bucketKey, string objectName, long contentLength, Stream stream)
        {
            var sessionId = Guid.NewGuid().ToString("N");
            const int bufferSize = (1 << 20) * 99;
            await using var ms = new MemoryStream(new byte[bufferSize], 0, bufferSize, true, true);

            var offset = 0L;
            string objectId = null;
            while (offset < contentLength)
            {
                var bytesRead = await stream.ReadAsync(ms.GetBuffer(), 0, bufferSize);
                if (bytesRead == 0)
                    throw new Exception($"Unexpected bytes read: 0");

                ms.Seek(0, SeekOrigin.Begin);
                ms.SetLength(bytesRead);

                var contentRange = $"bytes {offset}-{offset + bytesRead - 1}/{contentLength}";
                var response = await _objectApiRunner.PostOkOrAcceptedAsync(
                    api => api.UploadChunkAsyncWithHttpInfo(bucketKey, objectName, (int)contentLength, contentRange, sessionId, ms), 3);
                if (response.StatusCode == (int)HttpStatusCode.OK)
                {
                    var objectDetails = response.Data;
                    objectId = objectDetails.objectId;
                }

                offset += bytesRead;
            }

            if (objectId == null)
                throw new Exception("Object id is not returned");

            return objectId;
        }
    }
}