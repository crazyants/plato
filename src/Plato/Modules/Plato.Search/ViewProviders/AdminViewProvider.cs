using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plato.Internal.Layout.ViewProviders;
using Plato.Search.Models;
using Plato.Search.Repositories;
using Plato.Search.Stores;
using Plato.Search.ViewModels;

namespace Plato.Search.ViewProviders
{
    public class AdminViewProvider : BaseViewProvider<SearchSettings>
    {

        private readonly ISearchSettingsStore<SearchSettings> _searchSettingsStore;
        private readonly ICatalogRepository _catalogRepository;

        public AdminViewProvider(
            ISearchSettingsStore<SearchSettings> searchSettingsStore,
            ICatalogRepository catalogRepository)
        {
            _searchSettingsStore = searchSettingsStore;
            _catalogRepository = catalogRepository;
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(SearchSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(SearchSettings settings, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(SearchSettings settings, IViewProviderContext context)
        {

            var viewModel = await GetModel();
            return Views(
                View<SearchSettingsViewModel>("Admin.Edit.Header", model => viewModel).Zone("header").Order(1),
                View<SearchSettingsViewModel>("Admin.Edit.Tools", model => viewModel).Zone("tools").Order(1),
                View<SearchSettingsViewModel>("Admin.Edit.Content", model => viewModel).Zone("content").Order(1)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(SearchSettings settings,
            IViewProviderContext context)
        {
            var model = new SearchSettingsViewModel();

            // Validate model
            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(settings, context);
            }

            // Update settings
            if (context.Updater.ModelState.IsValid)
            {
                var result = await _searchSettingsStore.SaveAsync(new SearchSettings()
                {
                    SearchType = model.SearchType
                });
            }

            return await BuildEditAsync(settings, context);

        }

        async Task<SearchSettingsViewModel> GetModel()
        {

            var model = new SearchSettingsViewModel
            {
                AvailableSearchTypes = GetAvailableSearchTypes(),
                Catalogs = await _catalogRepository.SelectCatalogs()
            };

            var settings = await _searchSettingsStore.GetAsync();
            if (settings != null)
            {
                model.SearchType = settings.SearchType;

            }

            return model;

        }


        IEnumerable<SelectListItem> GetAvailableSearchTypes()
        {

            var output = new List<SelectListItem>();
            foreach (var searchType in SearchDefaults.AvailableSearchTypes)
            {
                output.Add(new SelectListItem
                {
                    Text = searchType.Name,
                    Value = System.Convert.ToString((int)searchType.Type)
                });
            }

            return output;

        }

    }

}
