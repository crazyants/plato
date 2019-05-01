using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Docs.Models;
using Plato.Entities.Models;

namespace Plato.Docs.ViewComponents
{
    
    public class DocTreeMenuViewComponent : ViewComponent
    {
        
        public DocTreeMenuViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(IEntity entity)
        {
            
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return Task.FromResult((IViewComponentResult) View((Doc) entity));

        }

    }

}
