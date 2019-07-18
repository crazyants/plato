using System.Threading.Tasks;
using Plato.Categories.Stores;
using Plato.Ideas.Categories.Models;
using Plato.Follows.Stores;
using Plato.Follows.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Ideas.Categories.Follow.ViewProviders
{

    public class CategoryViewProvider : BaseViewProvider<Category>
    {

        private readonly IFollowStore<Follows.Models.Follow> _followStore;
        private readonly ICategoryStore<Category> _categoryStore;
        private readonly IContextFacade _contextFacade;
 
        public CategoryViewProvider(
            IFollowStore<Follows.Models.Follow> followStore,
            ICategoryStore<Category> categoryStore,
            IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
            _categoryStore = categoryStore;
            _followStore = followStore;
        }

        public override Task<IViewProviderResult> BuildDisplayAsync(Category category, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildIndexAsync(Category category, IViewProviderContext context)
        {

            // Get category Id
            var categoryId = 0;
            if (category != null)
            {
                categoryId = category.Id;
            }
         
            // Get follow type
            var followType = categoryId == 0
                ? FollowTypes.AllCategories
                : FollowTypes.Category;

            // Get permission
            var permission = categoryId == 0
                ? Follow.Permissions.FollowIdeaCategories
                : Follow.Permissions.FollowIdeaCategory;

            // Get thingId if available
            var thingId = 0;
            if (categoryId > 0)
            {
                var existingCategory = await _categoryStore.GetByIdAsync(categoryId);
                if (existingCategory != null)
                {
                    thingId = existingCategory.Id;
                }
            }

            // Are we already following?
            var isFollowing = false;
            var currentUser = await _contextFacade.GetAuthenticatedUserAsync();
            if (currentUser != null)
            {
                var existingFollow = await _followStore.SelectByNameThingIdAndCreatedUserId(
                    followType.Name,
                    thingId,
                    currentUser.Id);
                if (existingFollow != null)
                {
                    isFollowing = true;
                }
            }

            return Views(
                View<FollowViewModel>("Follow.Display.Tools", model =>
                {
                    model.FollowType = followType;
                    model.ThingId = thingId;
                    model.IsFollowing = isFollowing;
                    model.Permission = permission;
                    return model;
                }).Zone("tools").Order(-4)
            );


        }

        public override Task<IViewProviderResult> BuildEditAsync(Category category, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Category category, IViewProviderContext context)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
