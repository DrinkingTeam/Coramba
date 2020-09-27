using System;
using System.Collections.Generic;
using System.Reflection;
using Coramba.Common.Reflection;
using Coramba.DataAccess.Base;
using Coramba.DataAccess.Ef.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Coramba.DataAccess.Ef.Base
{
    public class EfPrimaryKeyGetter<TDbContext, TModel> : IPrimaryKeyGetter<TModel>
        where TDbContext: DbContext
        where TModel : class
    {
        public IDbContextGetter<TDbContext> DbContextGetter { get; }

        public EfPrimaryKeyGetter(IDbContextGetter<TDbContext> dbContextGetter)
        {
            DbContextGetter = dbContextGetter ?? throw new ArgumentNullException(nameof(dbContextGetter));
        }

        public IEnumerable<MemberInfo> Get()
        {
            var entityType = DbContextGetter.Get().Model.FindEntityType(typeof(TModel));
            if (entityType == null)
                throw new Exception($"Type {typeof(TModel)} not in DbContext");

            var primaryKey = entityType.FindPrimaryKey();
            if (primaryKey == null || primaryKey.Properties.Count == 0)
                yield break;

            foreach (var keyProperty in primaryKey.Properties)
            {
                var memberInfo = typeof(TModel).GetPropertyCached(keyProperty.Name);

                if (memberInfo == null)
                    throw new Exception($"Key member {keyProperty.Name} for {typeof(TModel)} is not found");

                yield return memberInfo;
            }
        }
    }
}
