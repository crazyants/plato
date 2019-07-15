using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Plato.Search.Models;
using Plato.Search.Stores;

namespace Plato.Search.Configuration
{

    public class SearchOptionsConfiguration : IConfigureOptions<SearchOptions>
    {

        private readonly ISearchSettingsStore<SearchSettings> _searchSettingsStore;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SearchOptionsConfiguration(
            IServiceScopeFactory serviceScopeFactory,
            ISearchSettingsStore<SearchSettings> searchSettingsStore)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _searchSettingsStore = searchSettingsStore;
        }

        public void Configure(SearchOptions options)
        {

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var settings = _searchSettingsStore
                    .GetAsync()
                    .GetAwaiter()
                    .GetResult();

                if (settings != null)
                {
                    options.SearchType = settings.SearchType;
                }
            
            }

        }

    }

}
