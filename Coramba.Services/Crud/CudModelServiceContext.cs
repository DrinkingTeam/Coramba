using System;
using Coramba.Core.Converters;
using Coramba.DataAccess.Base;
using Coramba.DataAccess.Repositories;

namespace Coramba.Services.Crud
{
    public class CudModelServiceContext<TModel, TModelDto, TId>
    {
        public ICrudRepository<TModel> Repository { get; }
        public IModelFinder<TModel> ModelFinder { get; }
        public IObjectConverter<TModelDto, TId> DtoToIdConverter { get; }
        public IObjectConverter<TModelDto, TModel> DtoToModelConverter { get; }
        public IObjectConverter<TModel, TModelDto> ModelToDtoConverter { get; }
        public IQueryableEnumerator<TModel> QueryableEnumerator { get; }
        public ISelectOneModelService<TModelDto> SelectOneModelService { get; }

        public CudModelServiceContext(
            ICrudRepository<TModel> repository,
            IModelFinder<TModel> modelFinder,
            IObjectConverter<TModelDto, TId> dtoToIdConverter,
            IObjectConverter<TModelDto, TModel> dtoToModelConverter,
            IObjectConverter<TModel, TModelDto> modelToDtoConverter,
            IQueryableEnumerator<TModel> queryableEnumerator,
            ISelectOneModelService<TModelDto> selectOneModelService
            )
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));

            ModelFinder = modelFinder ?? throw new ArgumentNullException(nameof(modelFinder));
            DtoToIdConverter = dtoToIdConverter ?? throw new ArgumentNullException(nameof(dtoToIdConverter));
            DtoToModelConverter = dtoToModelConverter ?? throw new ArgumentNullException(nameof(dtoToModelConverter));
            ModelToDtoConverter = modelToDtoConverter ?? throw new ArgumentNullException(nameof(modelToDtoConverter));
            QueryableEnumerator = queryableEnumerator ?? throw new ArgumentNullException(nameof(queryableEnumerator));
            SelectOneModelService = selectOneModelService ?? throw new ArgumentNullException(nameof(selectOneModelService));
        }
    }
}
