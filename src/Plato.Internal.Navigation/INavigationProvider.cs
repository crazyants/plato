using System;
using System.Collections.Generic;
using System.Text;

namespace Plato.Internal.Navigation
{
    public interface INavigationProvider
    {
        void BuildNavigation(string name, NavigationBuilder builder);
    }
}
