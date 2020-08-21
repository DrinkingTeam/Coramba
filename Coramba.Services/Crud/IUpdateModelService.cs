using System.Threading.Tasks;

namespace Coramba.Services.Crud
{
    public interface IUpdateModelService<TModelDto>
    {
        Task<TModelDto> UpdateAsync(TModelDto modelDto);
    }
}
