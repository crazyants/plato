using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plato.Roles.ViewModels
{
    public class EditRoleViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        [DisplayName("role name")]
        public string RoleName { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        public string Description { get; set; }
        
        public bool IsNewRole { get; set; }


    }
}
