using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plato.Abstractions.Settings
{
    public interface ISiteService
    {
        /// <summary>
        /// Return the site settings for the current tenant.
        /// </summary>
        Task<ISite> GetSiteSettingsAsync();

        /// <summary>
        /// Persists the changes to the site settings.
        /// </summary>
        Task UpdateSiteSettingsAsync(ISite site);
    }
}
