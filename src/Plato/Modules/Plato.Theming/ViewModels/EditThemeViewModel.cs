using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Theming.ViewModels
{
    public class EditThemeViewModel
    {

        public string ThemeId { get; set; }

        public IEnumerable<SelectListItem> AvailableThemes { get; set; }
        
        public bool IsNewTheme { get; set; }

        public string Name { get; set; }

    }
}
