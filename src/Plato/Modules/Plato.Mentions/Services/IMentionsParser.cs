using System.Threading.Tasks;

namespace Plato.Mentions.Services
{

    public interface IMentionsParser
    {
        Task<string> ParseAsync(string input);
    }
    
}
