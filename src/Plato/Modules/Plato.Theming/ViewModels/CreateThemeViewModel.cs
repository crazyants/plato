using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Theming.ViewModels
{
    public class CreateThemeViewModel
    {

        public string Name { get; set; }

        public string ThemeId { get; set; }

        public IEnumerable<SelectListItem> AvailableThemes { get; set; }

    }

}
