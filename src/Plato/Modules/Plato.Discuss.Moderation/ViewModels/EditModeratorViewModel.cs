using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Plato.Discuss.Moderation.Models;
using Plato.Internal.Security.Abstractions;
using Plato.Moderation.Models;

namespace Plato.Discuss.Moderation.ViewModels
{
    public class EditModeratorViewModel
    {


        [Required]
        [Display(Name = "username")]
        public string Users { get; set; }

        public IDictionary<string, IEnumerable<ModeratorPermission>> CategorizedPermissions { get; set; }
        
        public IEnumerable<string> EnabledPermissions { get; set; }
        

        public bool EditAnyPost { get; set; }

        public bool DeleteAnyPost { get; set; }
        
        public OldModerator OldModerator { get; set; } = new OldModerator();

        public bool IsNewModerator { get; set; }

        public Moderator Moderator { get; set; }

    }
}
