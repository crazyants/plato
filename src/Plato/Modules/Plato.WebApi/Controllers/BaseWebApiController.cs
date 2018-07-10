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

    }
}
