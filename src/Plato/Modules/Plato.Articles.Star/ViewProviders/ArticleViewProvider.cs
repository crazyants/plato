using System.Threading.Tasks;
using Plato.Articles.Models;
using Plato.Stars.Stores;
using Plato.Stars.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Articles.Star.ViewProviders
{
    public class ArticleViewProvider : BaseViewProvider<Article>
    {
        
        private readonly IStarStore<Stars.Models.Star> _starStore;
        private readonly IContextFacade _contextFacade;
        
        public ArticleViewProvider(
            IStarStore<Stars.Models.Star> starStore,
            IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
            _starStore = starStore;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Article entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override async Task<IViewProviderResult> BuildDisplayAsync(Article entity, IViewProviderContext updater)
        {

            if (entity == null)
            {
                return await BuildIndexAsync(new Article(), updater);
            }

            var isStarred = false;
            var starType = StarTypes.Article;

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (user != null)
            {
                var entityStar = await _starStore.SelectByNameThingIdAndCreatedUserId(
                    starType.Name,
                    entity.Id,
                    user.Id);
                if (entityStar != null)
                {
                    isStarred = true;
                }
            }
            
            return Views(
                View<StarViewModel>("Star.Display.Tools", model =>
                {
                    model.StarType = starType;
                    model.ThingId = entity.Id;
                    model.IsStarred = isStarred;
                    model.TotalStars = entity.TotalStars;
                    model.Permission = Permissions.StarArticles;
                    return model;
                }).Zone("tools").Order(-5)
            );

        }

        public override Task<IViewProviderResult> BuildEditAsync(Article entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Article model, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

    }

}
