using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coramba.Services.Crud
{
    public interface ISelectAllModelService<TModelDto>
    {
        Task<IEnumerable<TModelDto>> SelectAllAsync();
    }
}
