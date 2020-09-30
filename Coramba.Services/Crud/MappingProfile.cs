using AutoMapper;
using Coramba.DataAccess.Queries.Universal;
using Coramba.Services.Filter;

namespace Coramba.Services.Crud
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap(typeof(FilterDto<>), typeof(UniversalFilter))
                .ForMember(nameof(UniversalFilter.Condition), x => x.Ignore())
                .ForMember(nameof(UniversalFilter.OrderBy), x => x.Ignore());
        }
    }
}
