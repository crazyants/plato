using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Plato.Discuss.Models;
using Plato.Discuss.Share.ViewModels;
using Plato.Discuss.ViewModels;
using Plato.Entities.Stores;
using Plato.Internal.Abstractions.Extensions;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Navigation;

namespace Plato.Discuss.Share.Controllers
{
    public class HomeController : Controller, IUpdateModel
    {

        private readonly IContextFacade _contextFacade;
        private readonly IEntityStore<Topic> _topicStore;
        private readonly IEntityReplyStore<Reply> _replyStore;
        private readonly IDbHelper _dbHelper;

        public HomeController(
            IEntityStore<Topic> topicStore,
            IEntityReplyStore<Reply> replyStore,
            IContextFacade contextFacade,
            IDbHelper dbHelper)
        {
            _topicStore = topicStore;
            _replyStore = replyStore;
            _contextFacade = contextFacade;
            _dbHelper = dbHelper;
        }

        // Share dialog

        public async Task<IActionResult> Index(int id, int replyId = 0)
        {

            // We always need an entity Id
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            // We always need an entity
            var entity = await _topicStore.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            
            // Optionally get reply
            Reply entityReply = null;
            if (replyId > 0)
            {
                entityReply = await _replyStore.GetByIdAsync(replyId);
            }
            
  
            var baseUrl = await _contextFacade.GetBaseUrlAsync();

            // Build view model
            var viewModel = new ShareViewModel();

            if (entityReply != null)
            {
                viewModel.EntityReplyUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
                {
                    ["Area"] = "Plato.Discuss.Share",
                    ["Controller"] = "Home",
                    ["Action"] = "Get",
                    ["Id"] = entity.Id,
                    ["Alias"] = entity.Alias,
                    ["ReplyId"] = entityReply.Id
                });
            }

            viewModel.EntityUrl = baseUrl + _contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["Area"] = "Plato.Discuss.Share",
                ["Controller"] = "Home",
                ["Action"] = "Get",
                ["Id"] = entity.Id,
                ["Alias"] = entity.Alias
            });
        
            return View(viewModel);

        }


        // Navigate to post

        public async Task<IActionResult> Get(int id, int replyId = 0)
        {

            // We always need an entity Id
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            // We always need an entity
            var entity = await _topicStore.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            
            // Optionally get reply
            Reply entityReply = null;
            if (replyId > 0)
            {
                entityReply = await _replyStore.GetByIdAsync(replyId);
            }
            
            // Get entity reply position
            var position = entityReply != null
                ? await GetEntityReplyPosition(entityReply)
                : 0;


            // Initialize default view model for replies to get default page size
            var topicOptions = new TopicViewModel
            {
                Pager = new PagerOptions()
            };

            var pageIndex = position.ToSafeCeilingDivision(topicOptions.Pager.PageSize);

            //pageIndex = Common.Utils.Numeric.TotalPages(lastPostPosition, pageSize);


            ViewBag.Position = position;
            ViewBag.PageIndex = pageIndex;

            return View();
        }


        async Task<int> GetEntityReplyPosition(Reply reply)
        {
            
            var sb = new StringBuilder();
            sb.Append("SELECT COUNT(Id) FROM {prefix}_EntityReplies");

            var where = BuildReplyPositionWhereClause(reply);
            if (!String.IsNullOrEmpty(where))
            {
                sb.Append(" WHERE ").Append(where);
            }
            
            return await _dbHelper.ExecuteScalarAsync<int>(sb.ToString());
            
        }

        string BuildReplyPositionWhereClause(Reply reply) 
        {
            var sb = new StringBuilder();
            sb.Append("EntityId = ")
                .Append(reply.EntityId)
                .Append(" AND ")
                .Append("Id <= ").Append(reply.Id);
            return sb.ToString();
        }

    }
}
