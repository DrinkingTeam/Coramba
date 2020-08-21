using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.Model;
using Coramba.DataAccess.Forge.Api;

namespace Coramba.DataAccess.Forge.Services
{
    class ForgeBucketsApiService : IForgeBucketsApiService
    {
        private readonly IForgeApiRunner<BucketsApi> _bucketApiRunner;

        public ForgeBucketsApiService(
            IForgeApiRunner<BucketsApi> bucketApiRunner)
        {
            _bucketApiRunner = bucketApiRunner;
        }

        public Task CreateBucketAsync(string bucketKey)
        {
            var bucket = new PostBucketsPayload(bucketKey, null, PostBucketsPayload.PolicyKeyEnum.Persistent);
            return _bucketApiRunner.PostOkAsync(api => api.CreateBucketAsyncWithHttpInfo(bucket));
        }
    }
}