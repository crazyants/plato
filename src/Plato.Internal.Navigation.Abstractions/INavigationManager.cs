using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Internal.Navigation.Abstractions
{
    public interface INavigationManager
    {
        IEnumerable<MenuItem> BuildMenu(string name, ActionContext context);
    }
}
