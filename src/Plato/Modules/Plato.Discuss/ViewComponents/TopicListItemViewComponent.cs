using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Entities.ViewModels;

namespace Plato.Discuss.ViewComponents
{
    public class TopicListItemViewComponent : ViewComponent
    {
  
        public TopicListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(EntityListItemViewModel<Topic> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

