using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.ViewModels;

namespace Plato.Discuss.ViewComponents
{
    public class TopicReplyListItemViewComponent : ViewComponent
    {
        
        public Task<IViewComponentResult> InvokeAsync(
            TopicReplyListItemViewModel model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

