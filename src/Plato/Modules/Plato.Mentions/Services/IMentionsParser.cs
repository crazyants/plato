using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Internal.Models.Users;

namespace Plato.Mentions.Services
{

    public interface IMentionsParser
    {

        /// <summary>
        /// Converts all @mentions to hyperlinks.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> ParseAsync(string input);

        /// <summary>
        /// Return all users @mentioned within supplied input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<IEnumerable<User>> GetUsersAsync(string input);

    }

}
