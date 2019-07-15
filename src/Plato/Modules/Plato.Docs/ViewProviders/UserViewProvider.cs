using System;
using System.Threading.Tasks;
using Plato.Docs.Models;
using Plato.Entities.Models;
using Plato.Entities.Services;
using Plato.Entities.ViewModels;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Users;

namespace Plato.Docs.ViewProviders
{

    public class UserViewProvider : BaseViewProvider<UserIndex>
    {

        private readonly IFeatureEntityCountService _featureEntityCountService;
        private readonly IPlatoUserStore<User> _platoUserStore;

        public UserViewProvider(
            IPlatoUserStore<User> platoUserStore,
            IFeatureEntityCountService featureEntityCountService)
        {
            _platoUserStore = platoUserStore;
            _featureEntityCountService = featureEntityCountService;
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(UserIndex userIndex, IViewProviderContext context)
        {

            var user = await _platoUserStore.GetByIdAsync(userIndex.Id);
            if (user == null)
            {
                return await BuildIndexAsync(userIndex, context);
            }
            
            var indexViewModel = context.Controller.HttpContext.Items[typeof(EntityIndexViewModel<Doc>)] as EntityIndexViewModel<Doc>;
            if (indexViewModel == null)
            {
                throw new Exception($"A view model of type {typeof(EntityIndexViewModel<Doc>).ToString()} has not been registered on the HttpContext!");
            }

            // Build feature entities model
            var featureEntityMetrics = new FeatureEntityCounts()
            {
                Features = await _featureEntityCountService
                    .ConfigureQuery(q =>
                    {
                        q.CreatedUserId.Equals(user.Id);
                        q.HideSpam.True();
                        q.HideHidden.True();
                        q.HideDeleted.True();
                        q.HidePrivate.True();

                    })
                    .GetResultsAsync()
            };

            // Build view model
            var userDisplayViewModel = new UserDisplayViewModel<Doc>()
            {
                User = user,
                IndexViewModel = indexViewModel,
                Counts = featureEntityMetrics
            };

            return Views(
                View<UserDisplayViewModel>("User.Index.Header", model => userDisplayViewModel).Zone("header"),
                View<UserDisplayViewModel<Doc>>("User.Index.Content", model => userDisplayViewModel).Zone("content"),
                View<UserDisplayViewModel>("User.Entities.Display.Sidebar", model => userDisplayViewModel).Zone("sidebar")
            );
            
        }

        public override Task<IViewProviderResult> BuildIndexAsync(UserIndex model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(UserIndex userIndex, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(UserIndex model, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
