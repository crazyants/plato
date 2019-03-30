using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Theming.ViewModels
{
    public class EditThemeViewModel
    {

        public string Theme { get; set; }

        public IEnumerable<SelectListItem> AvailableThemes { get; set; }


        public bool IsNewTheme { get; set; }

        public string Name { get; set; }

    }
}
