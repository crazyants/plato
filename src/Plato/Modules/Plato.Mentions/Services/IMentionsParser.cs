using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;

namespace Plato.Mentions.Services
{

    public interface IMentionsParser
    {
        Task<string> ParseAsync(string input);

        Task<IEnumerable<User>> GetUsersAsync(string input);

    }
    
}
