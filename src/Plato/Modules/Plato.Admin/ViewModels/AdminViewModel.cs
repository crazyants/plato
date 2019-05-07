using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Admin.ViewModels
{
    public class AdminViewModel
    {
        public IEnumerable<SelectListItem> FeatureCategories { get; set;  }
    }
}
