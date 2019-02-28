using System.ComponentModel.DataAnnotations;

namespace Plato.Entities.ViewModels
{
    public class EditEntityReplyViewModel
    {


        public int Id { get; set; }

        [Required]
        public int EntityId { get; set; }

        [Required]
        [MinLength(5)]
        [Display(Name = "message")]
        public string Message { get; set; }

        public string EditorHtmlName { get; set; }

    }

}
