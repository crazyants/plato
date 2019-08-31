using System.Threading.Tasks;
using Plato.Slack.Models;

namespace Plato.Slack.Services
{

    public interface ISlackService
    {
        Task<Response> PostAsync(string text);
    }

}
