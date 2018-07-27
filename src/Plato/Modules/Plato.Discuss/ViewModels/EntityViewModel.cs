using System.ComponentModel.DataAnnotations;

namespace Plato.Discuss.ViewModels
{

    public class EditEntityViewModel : NewEntityReplyViewModel
    {

        public int EntityId { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(255)]
        [Display(Name = "title")]
        public string Title { get; set; }

    }

    public class NewEntityReplyViewModel
    {
    
        [Required]
        [MinLength(5)]
        [Display(Name = "message")]
        public string Message { get; set; }

        public string EditorHtmlName { get; set; }

    }


}
