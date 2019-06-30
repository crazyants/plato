using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Plato.Moderation.Models;

namespace Plato.Discuss.Categories.Moderators.ViewModels
{
    public class EditModeratorViewModel
    {
        
        [Required]
        [Display(Name = "username")]
        public string Users { get; set; }

        public int UserId { get; set; }

        public IDictionary<string, IEnumerable<ModeratorPermission>> CategorizedPermissions { get; set; }
        
        public IEnumerable<string> EnabledPermissions { get; set; }
        
        public bool IsNewModerator { get; set; }

        public Moderator Moderator { get; set; }

        public IEnumerable<int> CategoryIds { get; set; }
    }
}
