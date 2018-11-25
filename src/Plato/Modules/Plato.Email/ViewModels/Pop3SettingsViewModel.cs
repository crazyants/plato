using System.ComponentModel.DataAnnotations;

namespace Plato.Email.ViewModels
{
    public class Pop3SettingsViewModel
    {

        public bool Enabled { get; set; }

        [Required]
        public string ServerName { get; set; }

    }

}
