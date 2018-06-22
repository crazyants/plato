using System.ComponentModel.DataAnnotations;

namespace Plato.Roles.ViewModels
{
    public class EditRoleViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text), MaxLength(255)]
        public string RoleName { get; set; }

    }
}
