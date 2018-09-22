using System.ComponentModel.DataAnnotations;

namespace Plato.Discuss.Labels.ViewModels
{
    public class EditLabelViewModel
    {

        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [StringLength(500)]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        [StringLength(50)]
        public string ForeColor { get; set; }

        [StringLength(50)]
        public string BackColor { get; set; }
  
        public bool IsNewLabel { get; set; }
        
    }
}
