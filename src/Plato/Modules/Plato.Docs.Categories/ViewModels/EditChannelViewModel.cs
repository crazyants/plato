using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plato.Categories.Models;

namespace Plato.Docs.Categories.ViewModels
{
    public class EditChannelViewModel
    {

        public bool IsNewChannel { get; set; }

        public int Id { get; set; }

        [Required]
        public int ParentId { get; set; }

        public IEnumerable<SelectListItem> AvailableChannels { get; set; }

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
        
        public string IconPrefix { get; set; }

        [StringLength(255)]
        public string IconCss { get; set; }
        
     
        public DefaultIcons ChannelIcons { get; set; }


    }
}
