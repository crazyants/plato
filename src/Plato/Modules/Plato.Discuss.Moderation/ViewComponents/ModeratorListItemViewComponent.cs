using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Moderation.Models;

namespace Plato.Discuss.Moderation.ViewComponents
{
    public class ModeratorListItemViewComponent : ViewComponent
    {
   
        public Task<IViewComponentResult> InvokeAsync(
            Moderator moderator)
        {
            return Task.FromResult((IViewComponentResult) View(moderator));
        }
    

    }


}

