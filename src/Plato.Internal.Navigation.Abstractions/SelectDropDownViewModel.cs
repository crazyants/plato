using System.Collections.Generic;
using System.Security;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Navigation.Abstractions
{

    public class SelectDropDownViewModel
    {

        public string HtmlName { get; set; }
        
        public string SelectedValue { get; set; }

        public SelectDropDown SelectDropDown { get; set; }
        
    }

    public class SelectDropDown
    {
        public string Title { get; set; }

        public IEnumerable<SelectDropDownItem> Items { get; set; }

        public bool Multiple { get; set; }

        public string CssClass { get; set; } =
            "dropdown-menu dropdown-menu-400 dropdown-menu-right anim anim-2x anim-scale-in";

        public string InnerCssClass { get; set; } = "min-w-200 max-h-300 overflow-auto";

    }

    public class SelectDropDownItem
    {

        public string Text { get; set; }

        public string Description { get; set; }

        public string Value { get; set; }
        
        public Permission Permission { get; set; }

    }

}
