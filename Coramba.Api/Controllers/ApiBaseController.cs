using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Coramba.Api.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("v{version:apiVersion}/[controller]")]
    [ServiceFilter(typeof(ModelValidationActionFilter))]
    public abstract class ApiBaseController: ControllerBase
    {
        protected ApiBaseController(ApiBaseControllerContext context)
        {
        }
    }
}
