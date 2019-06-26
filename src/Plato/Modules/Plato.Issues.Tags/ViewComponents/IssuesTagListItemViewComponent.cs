using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Issues.Tags.Models;
using Plato.Tags.ViewModels;

namespace Plato.Issues.Tags.ViewComponents
{

    public class IssuesTagListItemViewComponent : ViewComponent
    {

   
        public IssuesTagListItemViewComponent()
        {
        
        }
        public Task<IViewComponentResult> InvokeAsync(
            TagListItemViewModel<Tag> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
