using System.Collections.Generic;

namespace Plato.Roles.ViewModels
{

    public class SelectRolesViewModel
    {

        public IList<Selection<string>> SelectedRoles { get; set; }

        public string HtmlName { get; set; }

    }

    public class Selection<T>
    {

        public bool IsSelected { get; set; }

        public T Value { get; set; }

    }

}
