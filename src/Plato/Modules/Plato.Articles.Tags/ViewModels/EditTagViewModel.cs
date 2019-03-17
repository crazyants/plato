using System.ComponentModel.DataAnnotations;

namespace Plato.Articles.Tags.ViewModels
{
    public class EditTagViewModel
    {

        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [StringLength(500)]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        public bool IsNewTag { get; set; }
        
    }
}
