using System.Threading.Tasks;
using Plato.Core.Models;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Entities.ViewProviders
{
    public class HomeViewProvider : BaseViewProvider<HomeIndex>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            
            // Build view model
            var coreIndexViewModel = new CoreIndexViewModel()
            {
                Latest = new EntityIndexViewModel<Entity>()
                {
                    Options = new EntityIndexOptions()
                    {
                        Sort = SortBy.LastReply,
                        NoResultsText = "No new contributions"
                    },
                    Pager = new PagerOptions()
                    {
                        Page = 1,
                        Size = 10,
                        Enabled = false
                    }
                },
                Popular = new EntityIndexViewModel<Entity>()
                {
                    Options = new EntityIndexOptions()
                    {
                        Sort = SortBy.Popular,
                        NoResultsText = "No popular contributions"
                    },
                    Pager = new PagerOptions()
                    {
                        Page = 1,
                        Size = 10,
                        Enabled = false
                    }
                }
            };

            // Build view
            return Task.FromResult(Views(
                View<CoreIndexViewModel>("Core.Entities.Index.Content", model => coreIndexViewModel)
                    .Zone("content").Order(2)
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult)); ;
        }

    }

}
