using System.Threading.Tasks;

namespace Plato.Markdown.Services
{
    public interface IMarkdownParser
    {
        Task<string> Parse(string markdown);

    }

}
