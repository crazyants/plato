using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Hosting.Abstractions;

namespace Plato.Questions.Private.ViewComponents
{
  
    public class QuestionPrivateMenuViewComponent : ViewComponent
    {

        private readonly IContextFacade _contextFacade;
      
        public QuestionPrivateMenuViewComponent(IContextFacade contextFacade)
        {
            _contextFacade = contextFacade;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View(await _contextFacade.GetAuthenticatedUserAsync());
        }

    }

}
