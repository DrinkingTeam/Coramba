using System.Threading.Tasks;

namespace Coramba.Services.Crud
{
    public interface IDeleteModelService<TModelDto>
    {
        Task<bool> DeleteAsync(object id);
    }
}
