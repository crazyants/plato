using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.History.ViewModels;
using Plato.Discuss.Models;
using Plato.Entities.Stores;


namespace Plato.Discuss.History.ViewComponents
{
  
    public class HistoryMenuViewComponent : ViewComponent
    {
  
     
        public HistoryMenuViewComponent()
        {
      
       
        }

        public Task<IViewComponentResult> InvokeAsync(
            Topic topic,
            Reply reply)
        {
    
            var viewModel = new HistoryMenuViewModel()
            {
                Topic = topic,
                Reply = reply,
        
            };

            return Task.FromResult((IViewComponentResult) View(viewModel));
        }

    }

}
