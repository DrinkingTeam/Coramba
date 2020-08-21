using System.Threading.Tasks;

namespace Coramba.Services.Crud
{
    public interface ISelectOneModelService<TModelDto>
    {
        Task<TModelDto> SelectOneAsync(object id);
    }
}
