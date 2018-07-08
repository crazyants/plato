using System.ComponentModel.DataAnnotations;

namespace Plato.Users.Social.ViewModels
{
    public class EditSocialViewModel
    {

        [StringLength(255)]
        [DataType(DataType.Url)]
        public string FacebookUrl { get; set; }

        [StringLength(255)]
        [DataType(DataType.Url)]
        public string TwitterUrl { get; set; }

        [StringLength(255)]
        [DataType(DataType.Url)]
        public string YouTubeUrl { get; set; }


    }
}
