using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Plato.Internal.Data.Abstractions;

namespace Plato.Search.ViewModels
{

    public class SearchSettingsViewModel
    {

        [Required]
        public SearchTypes SearchType { get; set; }

        public IEnumerable<SelectListItem> AvailableSearchTypes { get; set; }

    }

}
