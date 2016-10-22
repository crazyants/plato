using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Plato.Repositories.Settings;
using Plato.Models.Settings;
using Plato.Abstractions.Settings;
using Microsoft.Extensions.Caching.Memory;

namespace Plato.Services.Settings
{
    public class SiteService : ISiteService
    {

        private string SiteCacheKey = "SiteSettings";
        private IDictionary<string, string> _settings;

        ISettingRepository<Setting> _settingRepository;
        IMemoryCache _memoryCache;

        public SiteService(
           ISettingRepository<Setting> settingRepository,
           IMemoryCache memoryCache)
        {
            _settingRepository = settingRepository;
            _memoryCache = memoryCache;
        }

        public async Task<ISiteSettings> GetSiteSettingsAsync()
        {

            throw new NotImplementedException();
         
            ISiteSettings site;

            if (!_memoryCache.TryGetValue(SiteCacheKey, out site))
            {

             
                _memoryCache.Set(SiteCacheKey, site);


            }



        }

         

        public Task UpdateSiteSettingsAsync(ISiteSettings site)
        {
            throw new NotImplementedException();
        }

    }
}
