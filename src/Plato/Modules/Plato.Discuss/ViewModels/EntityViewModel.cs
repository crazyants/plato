using System.ComponentModel.DataAnnotations;

namespace Plato.Discuss.ViewModels
{

    public class EditEntityViewModel : NewEntityReplyViewModel
    {

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

    }

    public class NewEntityReplyViewModel
    {

        public int EntityId { get; set; }

        [Required]
        public string Message { get; set; }

        public string EditorHtmlName { get; set; }

    }


}
