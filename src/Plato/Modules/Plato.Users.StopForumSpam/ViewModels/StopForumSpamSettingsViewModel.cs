using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace Plato.Users.StopForumSpam.ViewModels
{
    public class StopForumSpamSettingsViewModel
    {

        private static IEnumerable<AggressionLevel> _aggressionLevels = new List<AggressionLevel>()
        {
            new AggressionLevel()
            {
                Name = "Low",
                Description = "Only check against the users IP address appears",
                Tick = 0
            },
            new AggressionLevel()
            {
                Name = "Medium",
                Description = "Check against the users email address and IP address.",
                Tick = 33
            },
            new AggressionLevel()
            {
                Name = "High",
                Description = "Check against the users username, email address & IP address",
                Tick = 66
            },
            new AggressionLevel()
            {
                Name = "Custom",
                Description = "Check against the users username, email address & IP address",
                Tick = 100
            }
        };


        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        [Display(Name = "site key")]
        public string ApiKey { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "username threshold")]
        public int UserNameThreshold { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "email threshold")]
        public int EmailThreshold { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "ip address threshold")]
        public int IpAddressThreshold { get; set; }

        public AggressionLevel SelectedAggressionLevel { get; set; } = _aggressionLevels.First();

        public IEnumerable<AggressionLevel> AggressionLevels { get; set; } = _aggressionLevels;
   
    }
    
    [DataContract]
    public class AggressionLevel
    {

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "tick")]
        public short Tick { get; set; }
        
    }

}

