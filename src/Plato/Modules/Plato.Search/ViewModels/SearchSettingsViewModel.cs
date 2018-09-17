using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Plato.Search.ViewModels
{

    public class SearchSettingsViewModel
    {

        [Required]
        public int SearchMethod { get; set; }

        public IEnumerable<SelectListItem> AvailableSearchMethods { get; set; }

    }

}
