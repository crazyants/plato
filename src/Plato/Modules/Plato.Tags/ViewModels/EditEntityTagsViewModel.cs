using System.ComponentModel.DataAnnotations;
using Plato.Internal.Security.Abstractions;

namespace Plato.Tags.ViewModels
{

    public class EditEntityTagsViewModel
    {

        [Required, Display(Name = "tags")]
        public string Tags { get; set; }

        public string HtmlName { get; set; }

        public Permission Permission { get; set; }

    }

}
