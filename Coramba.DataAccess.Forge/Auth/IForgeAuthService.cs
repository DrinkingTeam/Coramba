using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.Client;

namespace Coramba.DataAccess.Forge.Auth
{
    public interface IForgeAuthService
    {
        Task<ForgeAuthResult> TryAuthTwoLeggedAsync(Configuration configuration, string clientId, string clientSecret, params Scope[] scopes);
    }
}