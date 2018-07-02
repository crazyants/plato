using System.Threading.Tasks;

namespace Plato.Markdown.Services
{
    public interface IMarkdownParser
    {
        Task<string> ParseAsync(string markdown);

    }

}
