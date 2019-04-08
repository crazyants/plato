using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;

namespace Plato.Discuss.ViewComponents
{
    public class TopicReplyListItemViewComponent : ViewComponent
    {
        
        public Task<IViewComponentResult> InvokeAsync(
            EntityReplyListItemViewModel<Topic, Reply> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

