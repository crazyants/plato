using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Navigation.Abstractions
{

    public class SelectDropDownViewModel
    {

        public virtual string HtmlName { get; set; }
        
        public virtual string SelectedValue { get; set; }

        public virtual SelectDropDown DropDown { get; set; }


        public virtual async Task BuildAsync(
            IAuthorizationService authorizationService,
            ClaimsPrincipal claimsPrincipal)
        {
            await BuildAsync(authorizationService, null, claimsPrincipal);
        }

        public virtual async Task BuildAsync(
            IAuthorizationService authorizationService,
            object resource,
            ClaimsPrincipal claimsPrincipal)
        {
            
            // Disable any selection we don't have permission to
            var firstPass = new List<SelectDropDownItem>();
            foreach (var item in this.DropDown.Items)
            {
                if (item.Permission != null)
                {
                    if (item.Checked)
                    {
                        if (!await authorizationService.AuthorizeAsync(claimsPrincipal, resource, item.Permission))
                        {
                            item.Checked = false;
                        }
                    }
                }
                firstPass.Add(item);
            }

            // If we don't have access to the default selection from our first pass
            // ensure the first available item we have permission to is selected on our second pass

            List<SelectDropDownItem> secondPass = null;
            var checkedItem = firstPass.FirstOrDefault(i => i.Checked == true);
            if (checkedItem == null)
            {
                var selectionSet = false;
                secondPass = new List<SelectDropDownItem>();
                foreach (var item in firstPass)
                {
                    if (!selectionSet)
                    {
                        if (item.Permission != null)
                        {
                            if (await authorizationService.AuthorizeAsync(claimsPrincipal, resource, item.Permission))
                            {
                                item.Checked = true;
                                selectionSet = true;
                            }
                        }
                        else
                        {
                            item.Checked = true;
                            selectionSet = true;
                        }
                    }

                    secondPass.Add(item);
                }
            }

            var selectedValue = "";
            var items = secondPass ?? firstPass;
            foreach (var item in items)
            {
                if (!this.DropDown.Multiple)
                {
                    if (item.Checked)
                    {
                        selectedValue = item.Value;
                        break;
                    }
                }
            }

            this.DropDown.Items = items;
            this.SelectedValue = selectedValue;

        }
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

        public bool Checked { get; set; }

    }

}
