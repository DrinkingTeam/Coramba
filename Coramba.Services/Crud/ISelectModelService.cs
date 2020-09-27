using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coramba.Services.Crud
{
    public interface ISelectModelService<TModelDto, TFilterDto>
    {
        Task<IEnumerable<TModelDto>> SelectAsync(TFilterDto filterDto);
    }
}
