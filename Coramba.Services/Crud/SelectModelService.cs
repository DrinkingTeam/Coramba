using System.Collections.Generic;
using System.Threading.Tasks;
using Coramba.Core.Converters;
using Coramba.DataAccess.Queries;
using Coramba.DataAccess.Repositories;

namespace Coramba.Services.Crud
{
    public class SelectModelService<TModel, TModelDto, TFilter, TFilterDto> : ISelectModelService<TModelDto, TFilterDto>
    {
        private readonly IModelSelector<TModel, TModelDto> _modelSelector;
        private readonly IObjectConverter<TFilterDto, TFilter> _filterToDtoConverter;
        private readonly IRepository<TModel> _repository;
        private readonly IQuery<TModel, TFilter> _query;

        public SelectModelService(
            IModelSelector<TModel, TModelDto> modelSelector,
            IObjectConverter<TFilterDto, TFilter> filterToDtoConverter,
            IRepository<TModel> repository,
            IQuery<TModel, TFilter> query)
        {
            _modelSelector = modelSelector;
            _filterToDtoConverter = filterToDtoConverter;
            _repository = repository;
            _query = query;
        }

        public async Task<IEnumerable<TModelDto>> SelectAsync(TFilterDto filterDto)
        {
            var filter = await _filterToDtoConverter.Convert(filterDto).ToAsync();
            var result = await _query.QueryAsync(await _repository.QueryAsync(), filter);
            return await _modelSelector.SelectAsync(result);
        }
    }
}