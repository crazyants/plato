using System.ComponentModel.DataAnnotations;

namespace Plato.Users.Social.ViewModels
{
    public class EditSocialViewModel
    {

        [DataType(DataType.Url), MaxLength(255)]
        public string FacebookUrl { get; set; }

        [DataType(DataType.Url), MaxLength(255)]
        public string TwitterUrl { get; set; }

        [DataType(DataType.Url), MaxLength(255)]
        public string YouTubeUrl { get; set; }


    }
}
