using System.ComponentModel.DataAnnotations;

namespace Plato.Entities.ViewModels
{

    public class EditEntityViewModel
    {

        public int Id { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(255)]
        [Display(Name = "title")]
        public string Title { get; set; }

        [Required]
        [MinLength(5)]
        [Display(Name = "message")]
        public string Message { get; set; }

        public string EditorHtmlName { get; set; }

        public string Alias { get; set; }

    }

}
