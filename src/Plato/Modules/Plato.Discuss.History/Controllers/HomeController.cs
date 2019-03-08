using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Discuss.History.ViewModels;
using Plato.Discuss.Models;
using Plato.Entities.History.Models;
using Plato.Entities.History.Services;
using Plato.Entities.History.Stores;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.Alerts;
using Plato.Internal.Text.Abstractions.Diff;
using Plato.Internal.Text.Abstractions.Diff.Models;

namespace Plato.Discuss.History.Controllers
{
    public class HomeController : Controller
    {

        private readonly IInlineDiffBuilder _inlineDiffBuilder;
        private readonly IEntityStore<Topic> _entityStore;
        private readonly IEntityReplyStore<Reply> _entityReplyStore;
        private readonly IEntityHistoryStore<EntityHistory> _entityHistoryStore;
        private readonly IEntityHistoryManager<EntityHistory> _entityHistoryManager;
        private readonly IContextFacade _contextFacade;
        private readonly IAlerter _alerter;

        public IHtmlLocalizer T { get; }

        public IStringLocalizer S { get; }

        public HomeController(
            IStringLocalizer stringLocalizer,
            IHtmlLocalizer localizer,
            IEntityHistoryStore<EntityHistory> entityHistoryStore,
            IInlineDiffBuilder inlineDiffBuilder,
            IEntityStore<Topic> entityStore,
            IAlerter alerter, IEntityReplyStore<Reply> entityReplyStore,
            IContextFacade contextFacade,
            IEntityHistoryManager<EntityHistory> entityHistoryManager)
        {
            _entityHistoryStore = entityHistoryStore;
            _inlineDiffBuilder = inlineDiffBuilder;
            _entityStore = entityStore;
            _alerter = alerter;
            _entityReplyStore = entityReplyStore;
            _contextFacade = contextFacade;
            _entityHistoryManager = entityHistoryManager;

            T = localizer;
            S = stringLocalizer;

        }

        // --------------
        // Version modal
        // --------------

        public async Task<IActionResult> Index(int id)
        {

            var history = await _entityHistoryStore.GetByIdAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            var entity = await _entityStore.GetByIdAsync(history.EntityId);
            if (entity == null)
            {
                return NotFound();
            }
            
            // Get previous history 
            var previousHistory = await _entityHistoryStore.QueryAsync()
                .Take(1)
                .Select<EntityHistoryQueryParams>(q =>
                {
                    q.Id.LessThan(history.Id);
                    q.EntityId.Equals(history.EntityId);
                    q.EntityReplyId.Equals(history.EntityReplyId);
                })
                .OrderBy("Id", OrderBy.Desc)
                .ToList();
            
            // Compare previous to current
            var html = history.Html;
            if (previousHistory?.Data != null)
            {
                html = PrepareDifAsync(previousHistory.Data[0].Html, history.Html);
            }
        
            // Build model
            var viewModel = new HistoryIndexViewModel()
            {
                History = history,
                Html = html
            };

            return View(viewModel);
        }

        // --------------
        // Delete version
        // --------------

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {

            // Validate
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id));
            }

            // Get history point
            var history = await _entityHistoryStore.GetByIdAsync(id);

            // Ensure we found the entity
            if (history == null)
            {
                return NotFound();
            }

            // Get entity
            var entity = await _entityStore.GetByIdAsync(history.EntityId);

            // Ensure we found the entity
            if (entity == null)
            {
                return NotFound();
            }

            // Get reply
            Reply reply = null;
            if (history.EntityReplyId > 0)
            {
                reply = await _entityReplyStore.GetByIdAsync(history.EntityReplyId);

                // Ensure we found a reply if supplied
                if (reply == null)
                {
                    return NotFound();
                }
            }
            
            // Delete history point
            var result = await _entityHistoryManager.DeleteAsync(history);

            // Add result
            if (result.Succeeded)
            {
                _alerter.Success(T["Version Deleted Successfully!"]);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    _alerter.Danger(T[error.Description]);
                }
            }
            
            // Redirect
            return Redirect(_contextFacade.GetRouteUrl(new RouteValueDictionary()
            {
                ["area"] = "Plato.Discuss",
                ["controller"] = "Home",
                ["action"] = "Reply",
                ["opts.id"] = entity.Id,
                ["opts.alias"] = entity.Alias,
                ["opts.replyId"] = reply?.Id ?? 0
            }));

        }

        string PrepareDifAsync(string before, string after)
        {

            var sb = new StringBuilder();
            var diff = _inlineDiffBuilder.BuildDiffModel(before, after);

            foreach (var line in diff.Lines)
            {
                switch (line.Type)
                {
                    case ChangeType.Inserted:
                        sb.Append("<div class=\"inserted-line\">");
                        sb.Append(line.Text);
                        sb.Append("</div>");
                        break;
                    case ChangeType.Deleted:
                        sb.Append("<div class=\"deleted-line\">");
                        sb.Append(line.Text);
                        sb.Append("</div>");
                        break;
                    case ChangeType.Imaginary:
                        sb.Append("<div class=\"imaginary-line\">");
                        sb.Append(line.Text);
                        sb.Append("</div>");
                        break;
                    case ChangeType.Modified:
                        foreach (var character in line.SubPieces)
                        {
                            if (character.Type == ChangeType.Imaginary)
                            {
                                continue;
                            }
                            sb.Append("<span class=\"")
                                .Append(character.Type.ToString().ToLower())
                                .Append("-character\">")
                                .Append(character.Text)
                                .Append("</span>");
                        }
                        break;
                    default:
                        sb.Append(line.Text);
                        break;
                }
                
            }

            return sb.ToString();

        }

    }

}
