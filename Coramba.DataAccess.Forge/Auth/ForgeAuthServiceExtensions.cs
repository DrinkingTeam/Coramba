using System;
using System.Net;
using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.Client;

namespace Coramba.DataAccess.Forge.Auth
{
    public static class ForgeAuthServiceExtensions
    {
        public static async Task<ForgeAuthResult> AuthTwoLeggedAsync(this IForgeAuthService forgeAuth, Configuration configuration, string clientId, string clientSecret, params Scope[] scopes)
        {
            var result = await forgeAuth.TryAuthTwoLeggedAsync(null, clientId, clientSecret, scopes);
            if (result.StatusCode < 0)
                throw result.Exception;

            if (result.StatusCode != (int)HttpStatusCode.OK)
                throw new Exception($"Login failed: status code = {result.StatusCode}");
            return result;
        }

        public static Task<ForgeAuthResult> AuthTwoLeggedAsync(this IForgeAuthService forgeAuth, string clientId, string clientSecret, params Scope[] scopes)
        {
            return forgeAuth.AuthTwoLeggedAsync(null, clientId, clientSecret, scopes);
        }
    }
}
