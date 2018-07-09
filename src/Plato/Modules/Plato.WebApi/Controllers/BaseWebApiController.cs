using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Plato.Internal.Models.Users;
using Plato.Internal.Security.Abstractions;
using Plato.WebApi.Services;

namespace Plato.WebApi.Controllers
{
    
    public class BaseWebApiController : Controller
    {

        public Task<User> GetAuthenticatedUserAsync()
        {
            var webApiAuthenticator = HttpContext.RequestServices.GetRequiredService<IWebApiAuthenticator>();
            return webApiAuthenticator.GetAuthenticatedUserAsync();
        }

    }
}
