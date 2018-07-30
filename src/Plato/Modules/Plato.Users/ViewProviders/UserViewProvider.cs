using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Navigation;
using Plato.Users.Models;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<UserProfile>
    {

        #region "Implementation"

        public override Task<IViewProviderResult> BuildIndexAsync(UserProfile user, IUpdateModel updater)
        {

            var filterOptions = new FilterOptions();

            var pagerOptions = new PagerOptions();
            pagerOptions.Page = GetPageIndex(updater);

            var viewModel = new UsersIndexViewModel()
            {
                FilterOpts = filterOptions,
                PagerOpts = pagerOptions
            };

            return Task.FromResult(
                Views(
                    View<UsersIndexViewModel>("Home.Index.Header", model => viewModel).Zone("header"),
                    View<UsersIndexViewModel>("Home.Index.Tools", model => viewModel).Zone("tools"),
                    View<UsersIndexViewModel>("Home.Index.Content", model => viewModel).Zone("content")
                ));


        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(UserProfile user, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(UserProfile user, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(UserProfile user, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        #endregion

        #region "Private Methods"

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

        #endregion

    }
}
