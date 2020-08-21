using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coramba.Core.Converters;

namespace Coramba.DataAccess.Common
{
    class QueryTransformer<TModelFrom, TModelTo> : IQueryTransformer<TModelFrom, TModelTo>
    {
        private readonly IObjectConverter<IQueryable<TModelFrom>, IEnumerable<TModelFrom>> _queryableToEnumerableConverter;
        private readonly IObjectConverter<TModelFrom, TModelTo> _modelConverter;

        public QueryTransformer(
            IObjectConverter<IQueryable<TModelFrom>, IEnumerable<TModelFrom>> queryableToEnumerableConverter,
            IObjectConverter<TModelFrom, TModelTo> modelConverter)
        {
            _queryableToEnumerableConverter = queryableToEnumerableConverter;
            _modelConverter = modelConverter;
        }

        public async Task<IEnumerable<TModelTo>> TransformAsync(IQueryable<TModelFrom> query)
        {
            return await TransformAsync(await _queryableToEnumerableConverter.Convert(query).ToAsync());
        }

        public async Task<IEnumerable<TModelTo>> TransformAsync(IEnumerable<TModelFrom> enumerable)
        {
            return await enumerable
                .ConvertItems(_modelConverter)
                .ToAsync();
        }
    }
}