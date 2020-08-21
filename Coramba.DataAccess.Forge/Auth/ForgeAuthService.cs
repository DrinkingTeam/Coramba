using System;
using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.Client;
using Microsoft.Extensions.Logging;

namespace Coramba.DataAccess.Forge.Auth
{
    public class ForgeAuthService : IForgeAuthService
    {
        public ILogger<ForgeAuthService> Logger { get; }

        public ForgeAuthService(ILogger<ForgeAuthService> logger)
        {
            Logger = logger;
        }

        public async Task<ForgeAuthResult> TryAuthTwoLeggedAsync(Configuration configuration, string clientId, string clientSecret, params Scope[] scopes)
        {
            var twoLeggedApi = new TwoLeggedApi(configuration);
            try
            {
                var result = await twoLeggedApi.AuthenticateAsyncWithHttpInfo(clientId, clientSecret, oAuthConstants.CLIENT_CREDENTIALS, scopes);

                return new ForgeAuthResult
                {
                    StatusCode = result.StatusCode,
                    TokenType = (string)result.Data.token_type,
                    AccessToken = (string)result.Data.access_token,
                    ExpiresIn = (int)result.Data.expires_in,
                };
            }
            catch (Exception e)
            {
                Logger.LogCritical(e, "Forge login failed");
                return new ForgeAuthResult
                {
                    StatusCode = -1,
                    Exception = e
                };
            }
        }
    }
}
