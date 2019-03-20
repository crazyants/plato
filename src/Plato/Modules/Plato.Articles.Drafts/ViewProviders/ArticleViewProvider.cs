using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Articles.Models;
using Plato.Articles.Drafts.ViewModels;

namespace Plato.Articles.Drafts.ViewProviders
{
    public class ArticleViewProvider : BaseViewProvider<Article>
    {

        private const string FollowHtmlName = "IsDraft";
        
        private readonly HttpRequest _request;
 
        public ArticleViewProvider(
            IContextFacade contextFacade,
            IHttpContextAccessor httpContextAccessor)
        {   
            _request = httpContextAccessor.HttpContext.Request;
        }
        
        public override Task<IViewProviderResult> BuildIndexAsync(Article entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }
        
        public override Task<IViewProviderResult> BuildDisplayAsync(Article entity, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildEditAsync(Article entity, IViewProviderContext updater)
        {
                      
            return Task.FromResult(Views(
                View<DraftViewModel>("Article.Draft.Edit.Sidebar", model =>
                {
                    model.IsPrivate = true;
                    return model;
                }).Zone("sidebar").Order(10)
            ));

        }

        public override Task<IViewProviderResult> BuildUpdateAsync(Article topic, IViewProviderContext updater)
        {


            return Task.FromResult(default(IViewProviderResult));

            //// Ensure entity exists before attempting to update
            //var entity = await _entityStore.GetByIdAsync(topic.Id);
            //if (entity == null)
            //{
            //    return await BuildEditAsync(topic, updater);
            //}

            //// Get the follow checkbox value
            //var follow = false;
            //foreach (var key in _request.Form.Keys)
            //{
            //    if (key == FollowHtmlName)
            //    {
            //        var values = _request.Form[key];
            //        if (!String.IsNullOrEmpty(values))
            //        {
            //            follow = true;
            //            break;
            //        }
            //    }
            //}

            //// We need to be authenticated to follow
            //var user = await _contextFacade.GetAuthenticatedUserAsync();
            //if (user == null)
            //{
            //    return await BuildEditAsync(topic, updater);
            //}

            //// The follow type
            //var followType = FollowTypes.Topic;
      
            //// Get any existing follow
            //var existingFollow = await _followStore.SelectByNameThingIdAndCreatedUserId(
            //    followType.Name,
            //    entity.Id,
            //    user.Id);
            
            //// Add the follow
            //if (follow)
            //{
            //    // If we didn't find an existing follow create a new one
            //    if (existingFollow == null)
            //    {
            //        // Add follow
            //        await _followManager.CreateAsync(new Follows.Models.Follow()
            //        {
            //            Name = followType.Name,
            //            ThingId = entity.Id,
            //            CreatedUserId = user.Id,
            //            CreatedDate = DateTime.UtcNow
            //        });
            //    }
      
            //}
            //else
            //{
            //    if (existingFollow != null)
            //    {
            //        await _followManager.DeleteAsync(existingFollow);
            //    }
            //}

            //return await BuildEditAsync(topic, updater);

        }

    }
}
