using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Plato.Internal.Models.Roles;
using Plato.Internal.Security.Abstractions;

namespace Plato.Roles.ViewModels
{
    public class EditRoleViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Text)]
        public string RoleName { get; set; }
        
        [StringLength(255)]
        [DataType(DataType.Text)]
        public string Description { get; set; }
        
        public bool IsNewRole { get; set; }
        
        public IDictionary<string, IEnumerable<Permission>> CategorizedPermissions { get; set; }

        public IEnumerable<string> EnabledPermissions { get; set; }

        public Role Role { get; set; }

    }
}
