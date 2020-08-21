using System.Threading.Tasks;
using Autodesk.Forge;

namespace Coramba.DataAccess.Forge.Auth
{
    public interface IForgeAuth
    {
        bool IsCredentialsSet();
        void SetCredentials(string apiKey, string apiSecret);
        void SetScopes(params Scope[] scopes);
        Task LoginAsync();
    }
}
