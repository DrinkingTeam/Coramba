using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Forge;

namespace Coramba.DataAccess.Forge.Auth
{
    class ForgeAuthCache : IForgeAuthCache
    {
        private readonly object _syncObject = new object();
        private readonly Dictionary<(string Key, string Secret), List<(Scope[] Scopes, DateTime Expiries, string Token)>> Tokens
            = new Dictionary<(string Key, string Secret), List<(Scope[] Scopes, DateTime Expiries, string Token)>>();

        private List<(Scope[] Scopes, DateTime Expiries, string Token)> GetTokens(string key, string secret)
        {
            if (Tokens.TryGetValue((key, secret), out var list))
            {
                for (var i = list.Count - 1; i >= 0; i--)
                    if (list[i].Expiries < DateTime.UtcNow)
                        list.RemoveAt(i);

                return list;
            }

            list = new List<(Scope[] Scopes, DateTime Expiries, string token)>();
            Tokens.Add((key, secret), list);
            return list;
        }

        private bool IsGood(Scope[] cacheScopes, Scope[] scopes)
        {
            return cacheScopes.Intersect(scopes).Count() == scopes.Length;
        }

        public string Get(string key, string secret, params Scope[] scopes)
        {
            lock (_syncObject)
            {
                var list = GetTokens(key, secret);
                return list.FirstOrDefault(x => IsGood(x.Scopes, scopes)).Token;
            }
        }

        public void Set(string key, string secret, Scope[] scopes, DateTime expires, string token)
        {
            lock (_syncObject)
            {
                GetTokens(key, secret).Add((scopes, expires, token));
            }
        }
    }
}