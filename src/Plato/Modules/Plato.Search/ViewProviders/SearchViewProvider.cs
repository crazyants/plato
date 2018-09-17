using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Search.ViewModels;
using Plato.WebApi.Controllers;

namespace Plato.Search.ViewProviders
{
    public class SearchViewProvider : BaseViewProvider<SearchResult>
    {
     
        public override Task<IViewProviderResult> BuildIndexAsync(SearchResult searchResult, IUpdateModel updater)
        {
            var viewModel = new SearchIndexViewModel
            {
                ViewOpts =
                {
                    Search = GetKeywords(updater)
                },
                PagerOpts =
                {
                    Page = GetPageIndex(updater)

                }
            };

            return Task.FromResult(Views(
                View<SearchIndexViewModel>("Home.Index.Header", model => viewModel).Zone("header"),
                View<SearchIndexViewModel>("Home.Index.Tools", model => viewModel).Zone("tools"),
                View<SearchIndexViewModel>("Home.Index.Sidebar", model => viewModel).Zone("sidebar").Order(3),
                View<SearchIndexViewModel>("Home.Index.Content", model => viewModel).Zone("content")
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(SearchResult model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }


        public override Task<IViewProviderResult> BuildEditAsync(SearchResult model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(SearchResult model, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        int GetPageIndex(IUpdateModel updater)
        {

            var page = 1;
            var routeData = updater.RouteData;
            var found = routeData.Values.TryGetValue("page", out object value);
            if (found)
            {
                int.TryParse(value.ToString(), out page);
            }

            return page;

        }

        string GetKeywords(IUpdateModel updater)
        {

            var keywords = string.Empty;
            var routeData = updater.RouteData;
            var found = routeData.Values.TryGetValue("search", out object value);
            if (found && value != null)
            {
                keywords = value.ToString();
            }

            return keywords;

        }



    }
}
