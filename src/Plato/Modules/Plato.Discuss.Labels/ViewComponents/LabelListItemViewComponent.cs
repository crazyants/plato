using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Labels.ViewModels;
using Plato.Discuss.Models;
using Plato.Entities.Stores;
using Plato.Internal.Hosting.Abstractions;
using Plato.Entities.Labels.Models;

namespace Plato.Discuss.Labels.ViewComponents
{
 
    public class LabelListItemViewComponent : ViewComponent
    {
        
        public LabelListItemViewComponent()
        {
        }

        public Task<IViewComponentResult> InvokeAsync(
            LabelListItemViewModel model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
