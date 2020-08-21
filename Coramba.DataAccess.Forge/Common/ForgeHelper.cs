using System.Collections.Generic;
using System.Linq;
using Autodesk.Forge.Model;

namespace Coramba.DataAccess.Forge.Common
{
    public static class ForgeHelper
    {
        public static DynamicDictionary ToDynamicDictionary(object obj)
            => ((DynamicDictionary) obj);

        public static Dictionary<string, object> ToDictionary(object obj)
            => ToDynamicDictionary(obj).Dictionary;

        public static IEnumerable<dynamic> ToEnumerable(object obj)
        {
            return ToDictionary(obj)
                .Select(x => x.Value);
        }
    }
}
