using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Navigation
{
    public interface INavigationProvider
    {
        void BuildNavigation(string name, NavigationBuilder builder);
    }
}
