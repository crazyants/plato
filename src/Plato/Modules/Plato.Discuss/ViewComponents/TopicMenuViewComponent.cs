using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Models;
using Plato.Discuss.Services;
using Plato.Discuss.ViewModels;
using Plato.Entities.Models;
using Plato.Internal.Navigation;

namespace Plato.Discuss.ViewComponents
{
    public class TopicMenuViewComponent : ViewComponent
    {

        private readonly INavigationManager _navigationManager;

        public TopicMenuViewComponent(INavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public Task<IViewComponentResult> InvokeAsync(Entity entity)
        {

            var items = _navigationManager.BuildMenu("topic", ViewContext);

            var viewModel = new TopicMenuViewModel()
            {
                Entity = entity
            };

            return Task.FromResult((IViewComponentResult) View(viewModel));

        }
        


    }

}
