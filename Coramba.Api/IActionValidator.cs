using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Coramba.Api
{
    public interface IActionValidator
    {
        Task ValidateAsync(ActionExecutingContext context);
    }
}
