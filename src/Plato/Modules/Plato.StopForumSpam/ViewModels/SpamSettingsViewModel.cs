using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.ViewModels
{
    public class SpamSettingsViewModel
    {
        
        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "site key")]
        public string ApiKey { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "spam level")]
        public int SpamLevelId { get; set; }

        // ----------

        public IEnumerable<SpamOperation> SpamOperations { get; set; }

        public IDictionary<string, IEnumerable<SpamOperation>> CategorizedSpamOperations { get; set; }

        public ISpamLevel SelectedSpamLevel
        {
            get { return SpamLevelDefaults.SpamLevels.FirstOrDefault(l => l.Id == SpamLevelId); }
        }

        public IEnumerable<ISpamLevel> SpamLevels { get; private set; } = SpamLevelDefaults.SpamLevels;
   
    }
    
}

