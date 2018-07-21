using System.ComponentModel.DataAnnotations;

namespace Plato.Discuss.ViewModels
{

    public class EditEntityViewModel : NewEntityReplyViewModel
    {

        [Required]
        [MinLength(5)]
        [MaxLength(255)]
        public string Title { get; set; }

    }

    public class NewEntityReplyViewModel
    {

        public int EntityId { get; set; }

        [Required]
        [MinLength(5)]
        public string Message { get; set; }

        public string EditorHtmlName { get; set; }

    }


}
