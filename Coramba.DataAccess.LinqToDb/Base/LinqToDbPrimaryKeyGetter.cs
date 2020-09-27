using System;
using System.Collections.Generic;
using System.Reflection;
using Coramba.DataAccess.Base;
using Coramba.DataAccess.LinqToDb.DataConnections;
using LinqToDB.Data;

namespace Coramba.DataAccess.LinqToDb.Base
{
    public class LinqToDbPrimaryKeyGetter<TDataConnection, TModel>: IPrimaryKeyGetter<TModel>
        where TDataConnection: DataConnection
        where TModel : class
    {
        public IDataConnectionGetter<TDataConnection> DbContextGetter { get; }

        public LinqToDbPrimaryKeyGetter(IDataConnectionGetter<TDataConnection> dbContextGetter)
        {
            DbContextGetter = dbContextGetter ?? throw new ArgumentNullException(nameof(dbContextGetter));
        }

        public IEnumerable<MemberInfo> Get()
        {
            var primaryKeys = DbContextGetter.Get().GetPkProperties(typeof(TModel));

            if (primaryKeys == null || primaryKeys.Length == 0)
                yield break;

            foreach (var keyProperty in primaryKeys)
                yield return keyProperty.MemberInfo;
        }
    }
}
