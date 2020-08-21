using System.Threading.Tasks;

namespace Coramba.Services.Crud
{
    public interface IInsertModelService<TModelDto>
    {
        Task<TModelDto> InsertAsync(TModelDto modelDto);
    }
}
