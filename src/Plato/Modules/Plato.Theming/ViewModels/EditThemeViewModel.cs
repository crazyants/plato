using System.Collections.Generic;
using Plato.Internal.Theming.Abstractions;

namespace Plato.Theming.ViewModels
{
    
    public class EditThemeViewModel
    {

        public string Id { get; set; }

        public IThemeFile File { get; set; }

        public IEnumerable<IThemeFile> Files { get; set; }

    }
    
}
