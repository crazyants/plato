using System.Threading.Tasks;

namespace Plato.Abstractions.Settings
{
    public interface ISiteService
    {

        Task<ISiteSettings> GetSiteSettingsAsync();

        Task<ISiteSettings> UpdateSiteSettingsAsync(ISiteSettings siteSettings);
    }
}
