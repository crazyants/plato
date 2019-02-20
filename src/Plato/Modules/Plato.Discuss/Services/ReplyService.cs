using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Plato.Discuss.Models;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Navigation;
using Plato.Internal.Security.Abstractions;

namespace Plato.Discuss.Services
{
    
    public class ReplyService : IReplyService
    {

        private readonly IEntityReplyStore<Reply> _entityReplyStore;
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReplyService(
            IEntityReplyStore<Reply> entityReplyStore,
            IAuthorizationService authorizationService,
            IHttpContextAccessor httpContextAccessor)
        {
            _entityReplyStore = entityReplyStore;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IPagedResults<Reply>> GetRepliesAsync(
            TopicOptions options,
            PagerOptions pager)
        {

            // Get principal
            var principal = _httpContextAccessor.HttpContext.User;


            return await _entityReplyStore.QueryAsync()
                .Take(pager.Page, pager.PageSize)
                .Select<EntityReplyQueryParams>(async q =>
                {
                    q.EntityId.Equals(options.Params.EntityId);

                    // Hide private?
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        Permissions.ViewPrivateReplies))
                    {
                        q.HidePrivate.True();
                    }

                    // Hide spam?
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        Permissions.ViewSpamReplies))
                    {
                        q.HideSpam.True();
                    }


                    // Hide deleted?
                    if (!await _authorizationService.AuthorizeAsync(principal,
                        Permissions.ViewDeletedReplies))
                    {
                        q.HideDeleted.True();
                    }
                    

                })
                .OrderBy(options.Sort, options.Order)
                .ToList();

        }

    }

}
