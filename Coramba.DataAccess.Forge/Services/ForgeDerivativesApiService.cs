using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Autodesk.Forge;
using Autodesk.Forge.Model;
using Coramba.Common;
using Coramba.DataAccess.Forge.Api;
using Coramba.DataAccess.Forge.Common;
using Microsoft.Extensions.Logging;

namespace Coramba.DataAccess.Forge.Services
{
    public class ForgeDerivativesApiService : IForgeDerivativesApiService
    {
        private const string GetPropertiesUriTemplate = "/modelderivative/v2/{region}designdata/{urn}/metadata/{guid}/properties?forceget=true";
        private const string GetPropertiesTreeUriTemplate = "/modelderivative/v2/{region}designdata/{urn}/metadata/{guid}?forceget=true";
        private const string GetManifestDerivativeUriTemplate = "/modelderivative/v2/{region}designdata/{urn}/manifest/{derivativeUrn}";
        private const string GetManifestDerivativeHeadUriTemplate = GetManifestDerivativeUriTemplate;
        private const string GetThumbnailUriTemplate = "/derivativeservice/v2/{region}thumbnails/{urn}";

        private readonly IForgeRawApiRunner _forgeRawApiRunner;
        private readonly IForgeApiRunner<DerivativesApi> _derivativesApiRunner;
        private readonly ILogger<ForgeDerivativesApiService> _logger;

        public ForgeDerivativesApiService(
            IForgeRawApiRunner forgeRawApiRunner,
            IForgeApiRunner<DerivativesApi> derivativesApiRunner,
            ILogger<ForgeDerivativesApiService> logger)
        {
            _forgeRawApiRunner = forgeRawApiRunner;
            _derivativesApiRunner = derivativesApiRunner;
            _logger = logger;
        }

        private async Task<bool> WaitOkAsync(Func<Task<HttpResponseMessage>> getResponse, Func<HttpResponseMessage, Task> onOk, TimeSpan? waitTime = null)
        {
            var attempts = 0;
            var elapsed = new Stopwatch();
            elapsed.Start();
            while (elapsed.Elapsed <= (waitTime ?? TimeSpan.MaxValue))
            {
                using var response = await getResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    await onOk(response);
                    return true;
                }

                attempts++;

                await Task.Delay(TimeSpan.FromSeconds(Math.Min(20, attempts)));
            }

            _logger.LogWarning("WaitOk timeout");

            return false;
        }

        private Task<bool> WaitAndDownloadAsync(Func<Task<HttpResponseMessage>> getResponse, Stream output, TimeSpan? waitTime = null)
            => WaitOkAsync(getResponse, r => r.Content.CopyToAsync(output), waitTime);

        private static string BuildUri(string uriTemplate, object parameters = null)
        {
            if (parameters == null)
                return uriTemplate;

            return TypeDescriptor
                .GetProperties(parameters)
                .Cast<PropertyDescriptor>()
                .Aggregate(uriTemplate, (s, p) => s.Replace($"{{{p.Name}}}", p.GetValue(parameters)?.ToString()));
        }

        private static string GetRegionByJobUrn(string jobUrn)
            => ForgeBase64Helper.Base64Decode(jobUrn).IndexOf("wipemea", StringComparison.InvariantCulture) > -1 ? "regions/eu/" : string.Empty;

        public async Task GetPropertiesAsync(string jobUrn, Guid modelViewGuid, Stream output)
        {
            var uri = BuildUri(GetPropertiesUriTemplate, new
            {
                region = GetRegionByJobUrn(jobUrn),
                urn = HttpUtility.UrlEncode(jobUrn),
                guid = HttpUtility.UrlEncode(modelViewGuid.ToString("D"))
            });

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-ads-force", "true");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");

            await WaitAndDownloadAsync(() => _forgeRawApiRunner.GetOkOrAcceptedAsync(uri, client), output);
        }

        public Task<dynamic> GetManifestAsync(string jobUrn)
            =>_derivativesApiRunner.GetOkAsync(api => api.GetManifestAsyncWithHttpInfo(jobUrn));

        public Task<dynamic> GetMetadataAsync(string jobUrn)
            => _derivativesApiRunner.GetOkAsync(api => api.GetMetadataAsyncWithHttpInfo(jobUrn));

        public async Task GetMetadataTreeAsync(string jobUrn, Guid guid, Stream output)
        {
            var uri = BuildUri(GetPropertiesTreeUriTemplate, new
            {
                region = GetRegionByJobUrn(jobUrn),
                urn = HttpUtility.UrlEncode(jobUrn),
                guid = HttpUtility.UrlEncode(guid.ToString("D"))
            });

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("x-ads-force", "true");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");

            await WaitAndDownloadAsync(() => _forgeRawApiRunner.GetOkOrAcceptedAsync(uri, client), output);
        }

        public Task<dynamic> TranslateAsync(JobPayload jobPayload, bool xAdsForce)
            => _derivativesApiRunner.PostOkAsync(api => api.TranslateAsyncWithHttpInfo(jobPayload, xAdsForce));

        public async Task<MemoryStream> GetThumbnailAsync(string urn, int? width, int? height)
        {
            if (!width.HasValue && !height.HasValue)
                throw new Exception($"Width or Height must be set");

            width ??= height;
            height ??= width;

            try
            {
                var guid = await GetManifestGuidAsync(urn, "3d", "geometry");
                if (guid == null)
                    return await GetDefaultThumbnailAsync();

                var uri = BuildUri(GetThumbnailUriTemplate, new
                {
                    region = GetRegionByJobUrn(urn),
                    urn = urn
                });

                var query = new Dictionary<string, string>();

                query["role"] = "rendered";
                query["guid"] = guid;

                query["width"] = width.Value.ToString(CultureInfo.InvariantCulture);
                query["height"] = height.Value.ToString(CultureInfo.InvariantCulture);

                if (query.Count > 0)
                    uri += $"?{string.Join("&", query.Select(x => $"{HttpUtility.HtmlEncode(x.Key)}={HttpUtility.HtmlEncode(x.Value)}"))}";

                using var client = new HttpClient();

                var ms = new MemoryStream();

                var ok = await WaitAndDownloadAsync(async () => await _forgeRawApiRunner.GetOkOrAcceptedAsync(uri, client), ms, TimeSpan.FromMinutes(1));
                if (!ok)
                    return await GetDefaultThumbnailAsync();

                ms.Position = 0;

                return ms;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Loading from modelservices failed");
            }

            return await GetDefaultThumbnailAsync();

            async Task<MemoryStream> GetDefaultThumbnailAsync()
            {
                return await _derivativesApiRunner.GetOkAsync(api => api.GetThumbnailAsyncWithHttpInfo(urn, width, height));
            }
        }

        public async Task GetManifestDerivativeAsync(string jobUrn, string derivativeUrn, Stream output, Func<(long DoneBytes, long TotalBytes), Task> onProgress)
        {
            const int bufferSize = 255 * (1 << 20);

            var headUri = BuildUri(GetManifestDerivativeHeadUriTemplate, new
            {
                region = GetRegionByJobUrn(jobUrn),
                urn = HttpUtility.UrlEncode(jobUrn),
                derivativeUrn = HttpUtility.UrlEncode(derivativeUrn)
            });

            var uri = BuildUri(GetManifestDerivativeUriTemplate, new
            {
                region = GetRegionByJobUrn(jobUrn),
                urn = HttpUtility.UrlEncode(jobUrn),
                derivativeUrn = HttpUtility.UrlEncode(derivativeUrn)
            });
            
            using var client = new HttpClient();

            var contentLength = 0L;
            await WaitOkAsync(() => _forgeRawApiRunner.HeadOkOrAcceptedAsync(headUri, client), r =>
            {
                if (!r.Content.Headers.ContentLength.HasValue)
                    throw new Exception($"Content-length expected");
                contentLength = r.Content.Headers.ContentLength.Value;
                return Task.CompletedTask;
            });

            _logger.LogInformation($"Start stream position: {output.Position}");
            for (var offset = 0L; offset < contentLength; offset += bufferSize)
            {
                client.DefaultRequestHeaders.Range = new RangeHeaderValue(offset, Math.Min(offset + bufferSize, contentLength) - 1);

                using var response = await _forgeRawApiRunner.GetPartialContentAsync(uri, client);
                await response.Content.CopyToAsync(output);
                var doneBytes = output.Position;

                _logger.LogInformation($"Position after {Math.Min(offset + bufferSize, contentLength)} byte(s): {output.Position}");

                if (onProgress != null)
                    await onProgress((doneBytes, contentLength));
            }
            _logger.LogInformation($"End stream position: {output.Position}");
        }

        private bool HasProperty(dynamic data, string propertyName)
        {
            return ForgeHelper.ToDictionary((object)data).ContainsKey(propertyName);
        }

        private void FindUrns(dynamic parent, Func<dynamic, bool> predicate, Func<dynamic, bool> onTrue)
        {
            foreach (var child in ForgeHelper.ToEnumerable(parent))
            {
                if (predicate(child) && onTrue(child))
                    return;

                if (HasProperty(child, "children"))
                    FindUrns(child.children, predicate, onTrue);
            }
        }

        public async Task<HashSet<string>> GetManifestUrnsAsync(string jobUrn, string role)
        {
            _logger.LogInformation($"Starting urn search for role = {role} in manifest with {jobUrn}");

            var manifest = await _derivativesApiRunner.GetOkAsync(api => api.GetManifestAsyncWithHttpInfo(jobUrn));
            var status = (string)manifest.status;
            if (status != "success")
                throw new Exception("Forge job is in not completed state");

            if (!HasProperty(manifest, "derivatives"))
                return new HashSet<string>();
            var derivatives = manifest.derivatives;

            var urns = new HashSet<string>();
            FindUrns(derivatives, (Func<dynamic, bool>)(x =>
            {
                return HasProperty(x, "role") && x.role == role;
            }), (Func<dynamic, bool>)(x =>
            {
                if (HasProperty(x, "urn"))
                    urns.Add(x.urn);

                return false;
            }));

            _logger.LogInformation($"Found urns:{urns.Select(x => "  " + Environment.NewLine + x).Flatten()}");

            return urns;
        }

        private async Task<string> GetManifestGuidAsync(string jobUrn, string role, string type)
        {
            _logger.LogInformation($"Starting guid search for role = {role}, type = {type} in manifest with {jobUrn}");

            var manifest = await _derivativesApiRunner.GetOkAsync(api => api.GetManifestAsyncWithHttpInfo(jobUrn));
            var status = (string)manifest.status;
            if (status != "success")
                throw new Exception("Forge job is in not completed state");

            if (!HasProperty(manifest, "derivatives"))
                return null;
            var derivatives = manifest.derivatives;

            string guid = null;
            FindUrns(derivatives, (Func<dynamic, bool>)(x =>
            {
                return
                        HasProperty(x, "role") && x.role == role
                        && HasProperty(x, "type") && x.type == type;
            }), (Func<dynamic, bool>)(x =>
            {
                if (HasProperty(x, "guid"))
                {
                    guid = x.guid;
                    return true;
                }

                return false;
            }));

            _logger.LogInformation($"Found guid:{guid}");

            return guid;
        }
    }
}