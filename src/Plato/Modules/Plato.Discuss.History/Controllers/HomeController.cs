using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.History.ViewModels;
using Plato.Discuss.Models;
using Plato.Entities.History.Models;
using Plato.Entities.History.Stores;
using Plato.Entities.Stores;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Text.Abstractions.Diff;
using Plato.Internal.Text.Abstractions.Diff.Models;

namespace Plato.Discuss.History.Controllers
{
    public class HomeController : Controller
    {

        private readonly IInlineDiffBuilder _inlineDiffBuilder;

        private readonly IEntityStore<Topic> _topicStore;
        private readonly IEntityHistoryStore<EntityHistory> _entityHistoryStore;

        public HomeController(
            IEntityHistoryStore<EntityHistory> entityHistoryStore,
            IInlineDiffBuilder inlineDiffBuilder,
            IEntityStore<Topic> topicStore)
        {
            _entityHistoryStore = entityHistoryStore;
            _inlineDiffBuilder = inlineDiffBuilder;
            _topicStore = topicStore;
        }

        public async Task<IActionResult> Index(int id)
        {

            var history = await _entityHistoryStore.GetByIdAsync(id);
            if (history == null)
            {
                return NotFound();
            }

            var entity = await _topicStore.GetByIdAsync(history.EntityId);
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
                    q.EntityId.Equals(entity.Id);
                })
                .OrderBy("Id", OrderBy.Desc)
                .ToList();


            // Compare previous to current
            // If no previous exists use current
            var html = history.Html;
            if (previousHistory?.Data != null)
            {
                html = PrepareDifAsync(
                    previousHistory.Data[0].Html,
                    history.Html);
            }

            var viewModel = new HistoryIndexViewModel()
            {
                History = history,
                Html = html
            };

            return View(viewModel);
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
                            if (character.Type == ChangeType.Imaginary) continue;
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
