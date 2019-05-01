using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Issues.Models;
using Plato.Entities.ViewModels;

namespace Plato.Issues.ViewComponents
{
    public class IssueListItemViewComponent : ViewComponent
    {
        
        public IssueListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            EntityListItemViewModel<Issue> model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }


}

