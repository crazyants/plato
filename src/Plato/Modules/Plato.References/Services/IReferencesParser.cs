using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Entities.Models;

namespace Plato.References.Services
{
    public interface IReferencesParser
    {

        string ReplacePattern { get; set; }

        Task<string> ParseAsync(string input);

        Task<IEnumerable<Entity>> GetEntitiesAsync(string input);

    }

}
