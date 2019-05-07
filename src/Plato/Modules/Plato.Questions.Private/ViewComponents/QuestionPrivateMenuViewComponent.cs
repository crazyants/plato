using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Hosting.Abstractions;
using Plato.Questions.Private.ViewModels;

namespace Plato.Questions.Private.ViewComponents
{
  
    public class QuestionPrivateMenuViewComponent : ViewComponent
    {
  
        public QuestionPrivateMenuViewComponent(IContextFacade contextFacade)
        {
        }

        public Task<IViewComponentResult> InvokeAsync(PrivateMenuViewModel model)
        {

            if (model == null)
            {
                model = new PrivateMenuViewModel();
            }

            return Task.FromResult((IViewComponentResult) View(model));

        }

    }

}
