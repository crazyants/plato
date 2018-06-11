using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Stores.Users;
using Plato.Internal.Models.Users;

namespace Plato.Hosting.Web
{
    public class ContextFacade : IContextFacade
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public ContextFacade(
            IHttpContextAccessor httpContextAccessor,
            IPlatoUserStore<User> platoUserStore)
        {
            _httpContextAccessor = httpContextAccessor;
            _platoUserStore = platoUserStore;
        }
        
        public async Task<User> GetAuthenticatedUserAsync()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var identity = user?.Identity;
            if ((identity != null) && (identity.IsAuthenticated))
            {
                return await _platoUserStore.GetByUserNameAsync(identity.Name);
            }
            return null;
        } 

    }
}
