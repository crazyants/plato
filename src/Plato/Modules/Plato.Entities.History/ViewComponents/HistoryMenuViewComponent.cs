using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.History.ViewModels;

namespace Plato.Entities.History.ViewComponents
{

    public class HistoryMenuViewComponent : ViewComponent
    {
  
        public HistoryMenuViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(HistoryMenuViewModel model)
        {          
            return Task.FromResult((IViewComponentResult) View(model));
        }

    }

}
