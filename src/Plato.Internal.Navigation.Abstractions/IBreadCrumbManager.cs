using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Plato.Internal.Navigation.Abstractions
{
    public interface IBreadCrumbManager
    {

        void Configure(Action<INavigationBuilder> configureBuilder);

        IEnumerable<MenuItem> BuildMenu(ActionContext actionContext);

    }

}
