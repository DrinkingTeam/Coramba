using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Autodesk.Forge.Model;

namespace Coramba.DataAccess.Forge.Services
{
    public interface IForgeDerivativesApiService
    {
        Task GetPropertiesAsync(string jobUrn, Guid modelViewGuid, Stream output);
        Task<dynamic> GetManifestAsync(string jobUrn);
        Task<dynamic> GetMetadataAsync(string jobUrn);
        Task GetMetadataTreeAsync(string jobUrn, Guid guid, Stream output);
        Task<dynamic> TranslateAsync(JobPayload jobPayload, bool xAdsForce);
        Task<MemoryStream> GetThumbnailAsync(string urn, int? width, int? height);
        Task GetManifestDerivativeAsync(string jobUrn, string derivativeUrn, Stream output, Func<(long DoneBytes, long TotalBytes), Task> onProgress = null);
        Task<HashSet<string>> GetManifestUrnsAsync(string jobUrn, string role);
    }
}
