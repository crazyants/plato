using System.ComponentModel.DataAnnotations;

namespace Plato.Users.ViewModels
{
    public class ConfirmEmailViewModel
    {
        [Required]
        public string UserIdentifier { get; set; }
    }
}
