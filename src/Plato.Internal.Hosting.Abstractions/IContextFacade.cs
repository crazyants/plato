using System.Threading.Tasks;
using Plato.Internal.Abstractions.Settings;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;

namespace Plato.Internal.Hosting.Abstractions
{
    public interface IContextFacade
    {

        Task<User> GetAuthenticatedUserAsync();

        Task<ShellModule> GetCurrentFeatureAsync();

        Task<ISiteSettings> GetSiteSettingsAsync();

        Task<string> GetBaseUrl();

    }
}
