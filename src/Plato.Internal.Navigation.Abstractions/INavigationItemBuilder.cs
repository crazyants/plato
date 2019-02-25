using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Navigation.Abstractions
{
    public interface INavigationItemBuilder : INavigationBuilder    {

        INavigationItemBuilder Caption(LocalizedString caption);

        INavigationItemBuilder Authority(string authority);

        INavigationItemBuilder Attributes(IDictionary<string, object> attributes);

        INavigationItemBuilder IconCss(string css);

        INavigationItemBuilder Order(int order);

        INavigationItemBuilder Url(string url);

        INavigationItemBuilder Culture(string culture);

        INavigationItemBuilder Id(string id);

        INavigationItemBuilder AddClass(string className);

        INavigationItemBuilder RemoveClass(string className);

        INavigationItemBuilder LinkToFirstChild(bool value);

        INavigationItemBuilder LocalNav();

        INavigationItemBuilder Local(bool value);

        INavigationItemBuilder Permission(IPermission permission);

        INavigationItemBuilder DividerCss(string css);

        INavigationItemBuilder View(string viewName, object model = null);

        INavigationItemBuilder Resource(object resource);
        
        INavigationItemBuilder Action(RouteValueDictionary values);

        INavigationItemBuilder Action(string actionName);

        INavigationItemBuilder Action(string actionName, string controllerName);

        INavigationItemBuilder Action(string actionName, string controllerName, object values);

        INavigationItemBuilder Action(string actionName, string controllerName, RouteValueDictionary values);

        INavigationItemBuilder Action(string actionName, string controllerName, string areaName);

        INavigationItemBuilder Action(string actionName, string controllerName, string areaName,
            RouteValueDictionary values);

    }

}
