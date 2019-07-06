using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Articles.Models;
using Plato.Entities.Stores;
using Plato.Stars.Services;
using Plato.Stars.Stores;
using Plato.Stars.ViewModels;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;

namespace Plato.Articles.Star.ViewProviders
{
    public class ArticleViewProvider : BaseViewProvider<Article>
    {

        private const string StarHtmlName = "star";

        private readonly IStarManager<Stars.Models.Star> _starManager;
        private readonly IStarStore<Stars.Models.Star> _starStore;
        private readonly IEntityStore<Article> _entityStore;
        private readonly IContextFacade _contextFacade;
        private readonly HttpRequest _request;
 
        public ArticleViewProvider(
            IStarManager<Stars.Models.Star> starManager,
            IHttpContextAccessor httpContextAccessor,
            IStarStore<Stars.Models.Star> starStore,
            IEntityStore<Article> entityStore,
            IContextFacade contextFacade)
        {
            _request = httpContextAccessor.HttpContext.Request;
            _contextFacade = contextFacade;
            _entityStore = entityStore;
            _starManager = starManager;
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
            var starType = StarTypes.Topic;

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
