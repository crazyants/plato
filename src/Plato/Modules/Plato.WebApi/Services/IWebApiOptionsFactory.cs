using System.Threading.Tasks;
using Plato.WebApi.Models;

namespace Plato.WebApi.Services
{
    public interface IWebApiOptionsFactory
    {
        Task<WebApiOptions> GetSettingsAsync();
    }

}
