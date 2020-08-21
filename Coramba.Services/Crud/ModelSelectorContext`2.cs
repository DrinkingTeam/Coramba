using System;
using Coramba.DataAccess.Base;
using Coramba.DataAccess.Common;
using Coramba.DataAccess.Repositories;

namespace Coramba.Services.Crud
{
    public class ModelSelectorContext<TModel, TModelDto>: ModelSelectorContext<TModel, TModelDto, object>
    {
        public IModelFinder<TModel> ModelFinder { get; }

        public ModelSelectorContext(IRepository<TModel> repository,
            IQueryTransformer<TModel, TModelDto> queryTransformer,
            IModelFinder<TModel> modelFinder
            )
            :base(repository, queryTransformer)
        {
            ModelFinder = modelFinder ?? throw new ArgumentNullException(nameof(modelFinder));
        }
    }
}
