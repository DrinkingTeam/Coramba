using System;
using System.Threading.Tasks;
using Coramba.Core.Converters;

namespace Coramba.Services.Common
{
    public class DtoToIdConverter<TModelDto, TId>: ImmutableObjectConverter<TModelDto, TId>
    {
        private readonly Func<TModelDto, TId> _idGetter;

        public DtoToIdConverter(Func<TModelDto, TId> idGetter)
        {
            _idGetter = idGetter ?? throw new ArgumentNullException(nameof(idGetter));
        }

        protected override Task<TId> ConvertToAsync(TModelDto source)
        {
            return Task.FromResult(_idGetter(source));
        }
    }
}
