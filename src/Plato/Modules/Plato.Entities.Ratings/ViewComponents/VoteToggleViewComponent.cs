using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Ratings.ViewModels;

namespace Plato.Entities.Ratings.ViewComponents
{
    public class VoteToggleViewComponent : ViewComponent
    {

        public Task<IViewComponentResult> InvokeAsync(VoteToggleViewModel model)
        {
            // Return view
            return Task.FromResult((IViewComponentResult) View(model));
        }

    }
}
