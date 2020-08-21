using System;
using Autodesk.Forge;

namespace Coramba.DataAccess.Forge.Auth
{
    public interface IForgeAuthCache
    {
        string Get(string key, string secret, Scope[] scopes);
        void Set(string key, string secret, Scope[] scopes, DateTime expires, string token);
    }
}
