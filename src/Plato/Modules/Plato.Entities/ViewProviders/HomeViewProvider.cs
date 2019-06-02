using System;
using System.Threading.Tasks;
using Plato.Core.Models;
using Plato.Entities.Models;
using Plato.Entities.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation.Abstractions;

namespace Plato.Entities.ViewProviders
{
    public class HomeViewProvider : BaseViewProvider<HomeIndex>
    {

        public override Task<IViewProviderResult> BuildIndexAsync(HomeIndex viewModel, IViewProviderContext context)
        {

            var latestViewModel = new EntityIndexViewModel<Entity>()
            {
                Options = new EntityIndexOptions()
                {
                    Sort = SortBy.LastReply,
                    Order = OrderBy.Desc
                },
                Pager = new PagerOptions()
                {
                    Page = 1,
                    Size = 5,
                    Enabled = false
                }
            };

            var popularViewModel = new EntityIndexViewModel<Entity>()
            {
                Options = new EntityIndexOptions()
                {
                    Sort = SortBy.LastReply,
                    Order = OrderBy.Desc
                },
                Pager = new PagerOptions()
                {
                    Page = 1,
                    Size = 5,
                    Enabled = false
                }
            };

            var coreIndexViewModel = new CoreIndexViewModel()
            {
                Latest = latestViewModel,
                Popular = popularViewModel
            };

            return Task.FromResult(Views(
                View<CoreIndexViewModel>("Core.Entities.Index.Content", model => coreIndexViewModel)
                    .Zone("content").Order(2)
            ));

        }

        public override Task<IViewProviderResult> BuildDisplayAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<IViewProviderResult> BuildEditAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            throw new NotImplementedException();
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(HomeIndex viewModel, IViewProviderContext context)
        {
            throw new NotImplementedException();
        }

    }



}
