using System.ComponentModel.DataAnnotations;

namespace Plato.Slack.ViewModels
{
    public class SlackSettingsViewModel
    {

        [StringLength(255), Display(Name = "web hook url"), DataType(DataType.Url)]
        public string WebHookUrl { get; set; }

    }

}
