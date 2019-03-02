using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Entities.ViewModels
{
    public class ReportEntityViewModel
    {

        public EntityOptions Options { get; set; }

        [Required]
        [Display(Name = "report reason")]
        public int ReportReason { get; set; }

        public IEnumerable<SelectListItem> AvailableReportReasons { get; set; }

    }

}
