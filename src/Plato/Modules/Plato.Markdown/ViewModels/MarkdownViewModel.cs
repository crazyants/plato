using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plato.Markdown.ViewModels
{
    public class MarkdownViewModel
    {

        public string Id { get; set; }

        [Required, MinLength(5), DisplayName("message"), DataType(DataType.MultilineText)]
        public string Value { get; set; }

        public string PlaceHolderText { get; set; }

        public string HtmlName { get; set; }

        public bool AutoFocus { get; set; }

        public int TabIndex { get; set; }

    }

}
