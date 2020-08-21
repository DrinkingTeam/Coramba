using System;
using Coramba.DataAccess.Common;
using Coramba.DataAccess.Repositories;

namespace Coramba.Services.Crud
{
    public class ModelSelectorContext<TModel, TModelDto, TId>
    {
        public IRepository<TModel> Repository { get; }
        public IQueryTransformer<TModel, TModelDto> QueryTransformer { get; }

        public ModelSelectorContext(IRepository<TModel> repository,
            IQueryTransformer<TModel, TModelDto> queryTransformer
            )
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            QueryTransformer = queryTransformer ?? throw new ArgumentNullException(nameof(queryTransformer));
        }
    }
}
