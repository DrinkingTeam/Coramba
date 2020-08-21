using System;
using Coramba.Core.Converters;
using Coramba.DataAccess;
using Coramba.DataAccess.Base;
using Coramba.DataAccess.Repositories;

namespace Coramba.Services.Common
{
    public class BaseModelServiceContext<TModel>
    {
        public ICrudRepository<TModel> Repository { get; }
        public IObjectConverterResolver ObjectConverterResolver { get; }
        public IQueryableEnumerator<TModel> QueryableEnumerator { get; }

        public BaseModelServiceContext(ICrudRepository<TModel> repository, IObjectConverterResolver objectConverterResolver, IQueryableEnumerator<TModel> queryableEnumerator)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            ObjectConverterResolver = objectConverterResolver ?? throw new ArgumentNullException(nameof(objectConverterResolver));
            QueryableEnumerator = queryableEnumerator ?? throw new ArgumentNullException(nameof(queryableEnumerator));
        }
    }
}
