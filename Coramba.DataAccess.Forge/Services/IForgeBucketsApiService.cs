using System.Threading.Tasks;

namespace Coramba.DataAccess.Forge.Services
{
    public interface IForgeBucketsApiService
    {
        Task CreateBucketAsync(string bucketKey);
    }
}
