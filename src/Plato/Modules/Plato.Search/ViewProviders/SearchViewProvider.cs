using System.Threading.Tasks;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Search.Models;
using Plato.Search.ViewModels;

namespace Plato.Search.ViewProviders
{
    public class SearchViewProvider : BaseViewProvider<SearchResult>
    {
     
        public override Task<IViewProviderResult> BuildIndexAsync(SearchResult searchResult, IViewProviderContext context)
        {
            
            var viewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel)] as EntityIndexViewModel;
            
            return Task.FromResult(Views(
                View<EntityIndexViewModel>("Home.Index.Header", model => viewModel).Zone("header"),
                View<EntityIndexViewModel>("Home.Index.Tools", model => viewModel).Zone("tools"),
                View<EntityIndexViewModel>("Home.Index.Sidebar", model => viewModel).Zone("sidebar").Order(3),
                View<EntityIndexViewModel>("Home.Index.Content", model => viewModel).Zone("content")
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(SearchResult model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }


        public override Task<IViewProviderResult> BuildEditAsync(SearchResult model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(SearchResult model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        

    }
}
