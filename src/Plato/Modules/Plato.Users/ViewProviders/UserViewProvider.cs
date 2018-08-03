using System.Threading.Tasks;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Users;
using Plato.Users.Models;
using Plato.Users.ViewModels;

namespace Plato.Users.ViewProviders
{
    public class UserViewProvider : BaseViewProvider<UserProfile>
    {

        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore)
        {
            _platoUserStore = platoUserStore;
        }

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
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(UserProfile userProfile, IUpdateModel updater)
        {

            var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userProfile, updater);
            }

            return Views(
                View<User>("Home.Display.Header", model => user).Zone("header"),
                View<User>("Home.Display.Tools", model => user).Zone("tools"),
                View<User>("Home.Display.Content", model => user).Zone("content"),
                View<User>("Home.Display.Sidebar", model => user).Zone("sidebar")
            );
        }

        public override async Task<IViewProviderResult> BuildEditAsync(UserProfile userProfile, IUpdateModel updater)
        {
            var user = await _platoUserStore.GetByIdAsync(userProfile.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userProfile, updater);
            }

            return Views(
                View<User>("Home.Edit.Header", model => user).Zone("header"),
                View<User>("Home.Edit.Tools", model => user).Zone("tools"),
                View<User>("Home.Edit.Content", model => user).Zone("content"),
                View<User>("Home.Edit.Footer", model => user).Zone("footer"),
                View<User>("Home.Edit.Sidebar", model => user).Zone("sidebar")
            );

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
