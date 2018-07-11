using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Users;
using Plato.WebApi.Services;

namespace Plato.WebApi.Controllers
{

    public class BaseWebApiController : Controller
    {

        public async Task<User> GetAuthenticatedUserAsync()
        {
            var webApiAuthenticator = HttpContext.RequestServices.GetRequiredService<IWebApiAuthenticator>();
            return await webApiAuthenticator.GetAuthenticatedUserAsync();
        }

        public ObjectResult UnauthorizedException()
        {
            return Result(
                HttpStatusCode.Unauthorized,
                "Could not authenticate the request. Please ensure you are logged in or have supplied a valid API key."
            );
        }

        public ObjectResult InternalServerError()
        {
            return Result(
                HttpStatusCode.InternalServerError,
                "An unhandled exception occurred whilst making the request."
            );
        }

        public ObjectResult NotFound()
        {
            return Result(
                HttpStatusCode.NotFound,
                "The resource you requested could not be found. It could be the resource has since been deleted."
            );
        }

        public ObjectResult Result(object result)
        {
            return new ObjectResult(new
            {
                result,
                StatusCode = HttpStatusCode.OK
            });
        }

        public ObjectResult Result(object result, string message)
        {
            return new ObjectResult(new
            {
                result,
                StatusCode = HttpStatusCode.OK,
                Message = message
            });
        }

        public ObjectResult Result(HttpStatusCode statusCode, string message)
        {
            return new ObjectResult(new
            {
                StatusCode = statusCode,
                Message = message
            });
        }

    }
}
