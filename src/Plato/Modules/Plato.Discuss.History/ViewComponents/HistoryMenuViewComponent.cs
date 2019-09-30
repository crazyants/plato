using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.History.ViewModels;
using Plato.Discuss.Models;

namespace Plato.Discuss.History.ViewComponents
{
  
    public class HistoryMenuViewComponent : ViewComponent
    {
  
        public HistoryMenuViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(Topic entity, Reply reply)
        {
    
            var viewModel = new HistoryMenuViewModel()
            {
                Topic = entity,
                Reply = reply,
            };

            return Task.FromResult((IViewComponentResult) View(viewModel));

        }

    }

}
