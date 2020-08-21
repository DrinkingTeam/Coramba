using System;
using System.Text;

namespace Coramba.DataAccess.Forge.Common
{
    public static class ForgeBase64Helper
    {
        public static string Base64Decode(string base64EncodedData)
        {
            string s = base64EncodedData.Replace('_', '/').Replace('-', '+');
            switch (base64EncodedData.Length % 4)
            {
                case 2:
                    s += "==";
                    break;
                case 3:
                    s += "=";
                    break;
            }
            return Encoding.UTF8.GetString(Convert.FromBase64String(s));
        }
    }
}
