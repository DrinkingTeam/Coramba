using System.Collections.Generic;
using System.Reflection;

namespace Coramba.DataAccess.Base
{
    public interface IPrimaryKeyGetter<T>
    {
        IEnumerable<MemberInfo> Get();
    }
}
