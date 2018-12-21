using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.History.ViewModels;
using Plato.Discuss.Models;
using Plato.Entities.History.Models;
using Plato.Entities.History.Stores;
using Plato.Entities.Stores;
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

            var html = PrepareDifAsync(entity.Html, history.Html);

            var viewModel = new HistoryIndexViewModel()
            {
                Html = history.Html
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
                        sb.Append("<span class=\"inserted\">");
                        sb.Append(line.Text);
                        sb.Append("</span>");
                        break;
                    case ChangeType.Deleted:
                        sb.Append("<span class=\"deleted\">");
                        sb.Append(line.Text);
                        sb.Append("</span>");
                        break;
                    default:
                        sb.Append(line.Text);
                        break;
                }

                Console.WriteLine(line.Text);
            }

            return sb.ToString();

        }

    }

}
