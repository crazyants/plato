using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Plato.Stores.Users;
using Plato.Models.Users;

namespace Plato.Hosting.Web
{
    public class ContextFacade : IContextFacade
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPlatoUserStore _platoUserStore;

        public ContextFacade(
            IHttpContextAccessor httpContextAccessor,
            IPlatoUserStore platoUserStore)
        {
            _httpContextAccessor = httpContextAccessor;
            _platoUserStore = platoUserStore;
        }
        
        public async Task<User> GetAuthenticatedUser()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var identity = user?.Identity;
            if ((identity != null) && (identity.IsAuthenticated))
            {
                return await _platoUserStore.GetByEmailAsync(identity.Name);
            }
            return null;
        } 

    }
}
