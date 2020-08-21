using System;

namespace Coramba.DataAccess.Forge.Auth
{
    public class ForgeAuthResult
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public int StatusCode { get; set; }
        public Exception Exception { get; set; }
    }
}
