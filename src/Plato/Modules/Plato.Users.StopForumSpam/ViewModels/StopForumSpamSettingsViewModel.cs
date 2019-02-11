using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using Plato.StopForumSpam.Models;

namespace Plato.Users.StopForumSpam.ViewModels
{
    public class StopForumSpamSettingsViewModel
    {

        private static IEnumerable<SpamLevel> _spamLevels = new List<SpamLevel>()
        {
            new SpamLevel()
            {
             
                Name = "Check against the users username, email address & IP address",
                Description = "All details must appear within the StopForumSpam database for the user to be flagged as SPAM. Some SPAM may get through but false positives will be reduced.",
                Frequencies = new SpamFrequencies()
                {
                    UserName = new SpamFrequency(1),
                    Email = new SpamFrequency(1),
                    IpAddress = new SpamFrequency(1)
                }
            },
            new SpamLevel()
            {
               
                Name = "Check against the users email address and IP address",
                Description = "The users email address & IP address must appear within the StopForumSpam database for the user to be flagged as SPAM. Usernames will be ignored. Some SPAM may get through but false positives based on username will be reduced.",
                Tick = 50,
                Frequencies = new SpamFrequencies()
                {
                    Email = new SpamFrequency(1),
                    IpAddress = new SpamFrequency(1)
                }
            },
            new SpamLevel()
            {
                Name = "Check against the users IP address only",
                Description = "If the users IP address appears within the StopForumSpam database the user will be flagged as SPAM. Catches the most SPAM but can lead to more false positives.",
                Tick = 100,
                Frequencies = new SpamFrequencies()
                {
                    IpAddress = new SpamFrequency(1)
                }
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

        public int SpamLevel { get; set; }

        public SpamLevel SelectedSpamLevel
        {
            get { return _spamLevels.FirstOrDefault(l => l.Tick == SpamLevel); }
        }
        public IEnumerable<SpamLevel> SpamLevels { get; private set; } = _spamLevels;
   
    }
    
    [DataContract]
    public class SpamLevel
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

