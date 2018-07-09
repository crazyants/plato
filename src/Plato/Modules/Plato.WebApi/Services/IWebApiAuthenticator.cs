using System.Threading.Tasks;
using Plato.Internal.Models.Users;

namespace Plato.WebApi.Services
{

    public interface IWebApiAuthenticator
    {
        Task<User> GetAuthenticatedUserAsync();
    }
    
}
