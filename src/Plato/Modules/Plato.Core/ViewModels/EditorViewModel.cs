using System.ComponentModel.DataAnnotations;

namespace Plato.Core.ViewModels
{
    public class EditorViewModel
    {

        public string Id { get; set;  }

        public string HtmlName { get; set; }

        public string Value { get; set; }

        public string PlaceHolderText { get; set; }


        public bool AutoFocus { get; set; }

    }

}
