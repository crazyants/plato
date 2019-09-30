using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.History.ViewModels;
using Plato.Entities.Models;

namespace Plato.Entities.History.ViewComponents
{
  
    public class HistoryMenuViewComponent : ViewComponent
    {
  
        public HistoryMenuViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(IEntity entity, IEntityReply reply)
        {

            var viewModel = new HistoryMenuViewModel()
            {
                Entity = entity,
                Reply = reply
            };

            return Task.FromResult((IViewComponentResult) View(viewModel));

        }

    }

}
