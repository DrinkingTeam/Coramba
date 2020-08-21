using System;
using System.Threading.Tasks;
using AutoMapper;
using Coramba.Core.Converters;

namespace Coramba.Core.AutoMapper
{
    public class AutoMapperObjectConverter<TSource, TDestination>: IObjectConverter<TSource, TDestination>
    {
        protected IMapper Mapper { get; }

        public AutoMapperObjectConverter(IMapper mapper)
        {
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public Task<TDestination> ConvertAsync(TSource id, TDestination destination)
        {
            return Task.FromResult(Mapper.Map(id, destination));
        }
    }
}
