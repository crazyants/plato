using System.ComponentModel.DataAnnotations;
using Plato.Discuss.Moderation.Models;

namespace Plato.Discuss.Moderation.ViewModels
{
    public class EditModeratorViewModel
    {


        [Required]
        [Display(Name = "username")]
        public string Users { get; set; }




        public bool EditAnyPost { get; set; }

        public bool DeleteAnyPost { get; set; }
        
        public OldModerator OldModerator { get; set; } = new OldModerator();

        public bool IsNewModerator { get; set; }

    }
}
