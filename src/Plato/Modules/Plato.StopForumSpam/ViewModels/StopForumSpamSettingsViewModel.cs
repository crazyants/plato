using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Plato.StopForumSpam.Client.Models;
using Plato.StopForumSpam.Models;

namespace Plato.StopForumSpam.ViewModels
{
    public class StopForumSpamSettingsViewModel
    {
        
        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "site key")]
        public string ApiKey { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "spam level")]
        public int SpamLevel { get; set; }

        // ----------

        public IEnumerable<SpamOperationType> SpamOperations { get; set; }

        public IDictionary<string, IEnumerable<SpamOperationType>> CategorizedSpamOperations { get; set; }

        public SpamLevel SelectedSpamLevel
        {
            get { return DefaultSpamLevels.SpamLevels.FirstOrDefault(l => l.Tick == SpamLevel); }
        }

        public IEnumerable<SpamLevel> SpamLevels { get; private set; } = DefaultSpamLevels.SpamLevels;
   
    }


    public interface ISpamLevel
    {
        string Name { get; set; }

        string Description { get; set; }

        short Tick { get; set; }

        SpamFrequencies Frequencies { get; set; }

    }
    
    [DataContract]
    public class SpamLevel : ISpamLevel
    {
        
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "tick")]
        public short Tick { get; set; }

        [DataMember(Name = "frequencies")]
        public SpamFrequencies Frequencies { get; set; }

    }
    
}

