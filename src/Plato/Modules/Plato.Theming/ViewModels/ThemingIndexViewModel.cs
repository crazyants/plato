using System.Collections.Generic;
using Plato.Internal.Theming.Abstractions.Models;

namespace Plato.Theming.ViewModels
{
    public class ThemingIndexViewModel
    {

        public IEnumerable<IThemeDescriptor> Themes { get; set; }

    }
}
