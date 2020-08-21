using System;
using System.Threading.Tasks;
using Autodesk.Forge;
using Autodesk.Forge.Client;
using Coramba.Common;
using Microsoft.Extensions.Logging;

namespace Coramba.DataAccess.Forge.Auth
{
    public class ForgeAuth: IForgeAuth
    {
        private string ApiKey { get; set; }
        private string ApiSecret { get; set; }
        private Scope[] Scopes { get; set; }
        private Configuration Configuration { get; }
        private ILogger<ForgeAuth> Logger { get; }
        public IForgeAuthCache ForgeAuthCache { get; }
        public IForgeAuthService ForgeAuthService { get; }

        public ForgeAuth(Configuration configuration, ILogger<ForgeAuth> logger, IForgeAuthCache forgeAuthCache, IForgeAuthService forgeAuthService)
        {
            Configuration = configuration;
            Logger = logger;
            ForgeAuthCache = forgeAuthCache;
            ForgeAuthService = forgeAuthService;
        }

        public bool IsCredentialsSet()
        {
            return ApiKey != null;
        }

        public bool IsScopesSet()
        {
            return Scopes != null;
        }

        public void SetCredentials(string apiKey, string apiSecret)
        {
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            ApiSecret = apiSecret ?? throw new ArgumentNullException(nameof(apiSecret));
        }

        public void SetScopes(params Scope[] scopes)
        {
            if (scopes == null) throw new ArgumentNullException(nameof(scopes));
            if (scopes.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(scopes));

            Scopes = scopes;
        }

        public async Task LoginAsync()
        {
            if (!IsCredentialsSet())
                throw new Exception($"Credentials should be set");

            if (!IsScopesSet())
                throw new Exception($"Scopes should be set");

            var accessToken = ForgeAuthCache.Get(ApiKey, ApiSecret, Scopes);
            if (accessToken == null)
            {
                var now = DateTime.UtcNow;
                Logger.LogInformation($"Forge auth starting: {ApiKey}, scopes = {Scopes.Flatten(",")}");
                
                var result = await ForgeAuthService.AuthTwoLeggedAsync(Configuration, ApiKey, ApiSecret, Scopes);
                accessToken = result.AccessToken;
                var expiresDateTime = now.AddSeconds(result.ExpiresIn - 10);

                ForgeAuthCache.Set(ApiKey, ApiSecret, Scopes, expiresDateTime, accessToken);
                Logger.LogInformation($"Forge auth success: {result.TokenType} {accessToken}");
            }

            Configuration.AccessToken = accessToken;
        }
    }
}
