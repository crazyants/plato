using System.ComponentModel.DataAnnotations;
using Plato.Categories.Models;

namespace Plato.Discuss.Channels.ViewModels
{
    public class EditChannelViewModel
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

        [StringLength(255)]
        public string IconCss { get; set; }
        
     
        public DefaultIcons ChannelIcons { get; set; }


    }
}
