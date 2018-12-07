using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Discuss.Tags.ViewModels;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Features.Abstractions;
using Plato.Internal.Navigation;
using Plato.Tags.Models;
using Plato.Tags.Stores;

namespace Plato.Discuss.Tags.ViewComponents
{

    public class TagListItemViewComponent : ViewComponent
    {

   
        public TagListItemViewComponent()
        {
        
        }
        public Task<IViewComponentResult> InvokeAsync(
            TagListItemViewModel model)
        {
            return Task.FromResult((IViewComponentResult)View(model));
        }

    }

}
