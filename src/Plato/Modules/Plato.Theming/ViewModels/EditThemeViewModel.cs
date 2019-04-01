using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Plato.Internal.Theming.Abstractions;

namespace Plato.Theming.ViewModels
{
    
    public class EditThemeViewModel
    {

        [Required]
        public string ThemeId { get; set; }

        public string Path { get; set; }

        [Required, DataType(DataType.MultilineText)]
        public string FileContents { get; set; }


        public IThemeFile File { get; set; }

        public IEnumerable<IThemeFile> Files { get; set; }

    }
    
}
