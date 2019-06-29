using System.ComponentModel.DataAnnotations;

namespace Plato.Twitter.ViewModels
{
    public class TwitterSettingsViewModel
    {

        [StringLength(255), Display(Name = "consumer key")]
        public string ConsumerKey { get; set; }

        [StringLength(255), Display(Name = "consumer secret")]
        public string ConsumerSecret { get; set; }
        
        [StringLength(255), Display(Name = "access token")]
        public string AccessToken { get; set; }

        [StringLength(255), Display(Name = "access token secret")]
        public string AccessTokenSecret { get; set; }

    }

}
