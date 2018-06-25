using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Roles.ViewModels;

namespace Plato.Roles.ViewComponents
{
    public class SelectRolesViewComponent : ViewComponent
    {
        private readonly IPlatoRoleStore _platoRoleStore;

        public SelectRolesViewComponent(
            IPlatoRoleStore platoRoleStore)
        {
            _platoRoleStore = platoRoleStore;
        }

        public async Task<IViewComponentResult> InvokeAsync(
            IEnumerable<string> selectedRoles, 
            string htmlName)
        {
            if (selectedRoles == null)
            {
                selectedRoles = new string[0];
            }

            var roleSelections = await BuildRoleSelectionsAsync(selectedRoles);

            var model = new SelectRolesViewModel
            {
                HtmlName = htmlName,
                SelectedRoles = roleSelections
            };

            return View(model);
        }

        private async Task<IList<Selection<string>>> BuildRoleSelectionsAsync(
            IEnumerable<string> selectedRoles)
        {
            var roleNames = await _platoRoleStore.GetRoleNamesAsync();
            var selections = roleNames.Select(r => new Selection<string>
                {
                    IsSelected = selectedRoles.Any(v => v == r),
                    Value = r
                })
                .OrderBy(r => r.Value)
                .ToList();

            return selections;
        }
    }


}

