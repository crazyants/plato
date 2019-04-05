using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Plato.Entities.Services;
using Plato.Entities.Stores;
using Plato.Entities.ViewModels;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Docs.ViewComponents
{

    public class DocCommentListViewComponent : ViewComponent
    {

        private readonly IEntityStore<Doc> _entityStore;
        private readonly IEntityReplyStore<DocComment> _entityReplyStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEntityReplyService<DocComment> _replyService;

        public DocCommentListViewComponent(
            IEntityReplyStore<DocComment> entityReplyStore,
            IEntityStore<Doc> entityStore,
            IEntityReplyService<DocComment> replyService, 
            IAuthorizationService authorizationService)
        {
            _entityReplyStore = entityReplyStore;
            _entityStore = entityStore;
            _replyService = replyService;
            _authorizationService = authorizationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            EntityOptions options,
            PagerOptions pager)
        {

            if (options == null)
            {
                options = new EntityOptions();
            }

            if (pager == null)
            {
                pager = new PagerOptions();
            }
            
            return View(await GetViewModel(options, pager));

        }

        async Task<EntityViewModel<Doc, DocComment>> GetViewModel(
            EntityOptions options,
            PagerOptions pager)
        {

            var entity = await _entityStore.GetByIdAsync(options.Id);
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            var results = await _replyService
                .ConfigureQuery(async q =>
                {

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewPrivateDocComments))
                    {
                        q.HidePrivate.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewSpamDocComments))
                    {
                        q.HideSpam.True();
                    }

                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(HttpContext.User,
                        Permissions.ViewDeletedDocComments))
                    {
                        q.HideDeleted.True();
                    }

                })
                .GetResultsAsync(options, pager);
            
            // Set total on pager
            pager.SetTotal(results?.Total ?? 0);

            // Return view model
            return new EntityViewModel<Doc, DocComment>
            {
                Options = options,
                Pager = pager,
                Entity = entity,
                Replies = results
        };

        }

    }

}

