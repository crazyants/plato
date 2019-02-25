using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Plato.Internal.Navigation.Abstractions
{
    public interface INavigationBuilder
    {

        ActionContext ActionContext { get; set; }

        List<MenuItem> Build();

        INavigationBuilder Add(LocalizedString caption, string position, Action<INavigationItemBuilder> itemBuilder,
            IEnumerable<string> classes = null);

        INavigationBuilder Add(LocalizedString caption, string authority, int order,
            Action<INavigationItemBuilder> itemBuilder, IEnumerable<string> classes = null);

        INavigationBuilder Add(LocalizedString caption, int order, Action<INavigationItemBuilder> itemBuilder,
            IEnumerable<string> classes = null);

        INavigationBuilder Add(LocalizedString caption, Action<INavigationItemBuilder> itemBuilder,
            IEnumerable<string> classes = null);

        INavigationBuilder Add(Action<INavigationItemBuilder> itemBuilder, IEnumerable<string> classes = null);

        INavigationBuilder Add(LocalizedString caption, string position, IEnumerable<string> classes = null);

        INavigationBuilder Add(LocalizedString caption, IEnumerable<string> classes = null);

        INavigationBuilder Remove(MenuItem item);

        INavigationBuilder Remove(Predicate<MenuItem> match);

    }

}
