using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Internal.Stores.Abstractions.Roles;

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

        public async Task<IViewComponentResult> InvokeAsync(IEnumerable<string> selectedRoles, string htmlName)
        {
            if (selectedRoles == null)
            {
                selectedRoles = new string[0];
            }

            var roleSelections = await BuildRoleSelectionsAsync(selectedRoles);

            var model = new SelectRolesViewModel
            {
                HtmlName = htmlName,
                RoleSelections = roleSelections
            };

            return View(model);
        }

        private async Task<IList<Selection<string>>> BuildRoleSelectionsAsync(IEnumerable<string> selectedRoles)
        {
            var roleNames = await _platoRoleStore.GetRoleNamesAsync();
            var selections = roleNames.Select(r => new Selection<string>
                {
                    IsSelected = selectedRoles.Contains(r),
                    Item = r
                })
                .OrderBy(r => r.Item)
                .ToList();

            return selections;
        }
    }

    public class SelectRolesViewModel
    {

        public string HtmlName { get; set; }

        public IList<Selection<string>> RoleSelections { get; set; }

    }

    public class Selection<T>
    {

        public bool IsSelected { get; set; }

        public T Item { get; set; }

    }
}

