using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;
using Plato.Internal.Navigation.Abstractions;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Navigation
{
    public class NavigationItemBuilder : NavigationBuilder, INavigationItemBuilder
    {
        private readonly MenuItem _item;

        public NavigationItemBuilder()
        {
            _item = new MenuItem();
        }

        public NavigationItemBuilder(MenuItem existingItem)
        {
            _item = existingItem ?? throw new ArgumentNullException(nameof(existingItem));
        }

        public INavigationItemBuilder Caption(LocalizedString caption)
        {
            _item.Text = caption;
            return this;
        }

        public INavigationItemBuilder Authority(string authority)
        {
            _item.Authority = authority;
            return this;
        }

        public INavigationItemBuilder Attributes(IDictionary<string, object> attributes)
        {
            _item.Attributes = attributes;
            return this;
        }

        public INavigationItemBuilder IconCss(string css)
        {
            _item.IconCss = css;
            return this;
        }

        public INavigationItemBuilder Order(int order)
        {
            _item.Order = order;
            return this;
        }
        
        public INavigationItemBuilder Url(string url)
        {
            _item.Url = url;
            return this;
        }

        public INavigationItemBuilder Culture(string culture)
        {
            _item.Culture = culture;
            return this;
        }

        public INavigationItemBuilder Id(string id)
        {
            _item.Id = id;
            return this;
        }

        public INavigationItemBuilder AddClass(string className)
        {
            if (!_item.Classes.Contains(className))
                _item.Classes.Add(className);
            return this;
        }

        public INavigationItemBuilder RemoveClass(string className)
        {
            if (_item.Classes.Contains(className))
                _item.Classes.Remove(className);
            return this;
        }

        public INavigationItemBuilder LinkToFirstChild(bool value)
        {
            _item.LinkToFirstChild = value;
            return this;
        }

        public INavigationItemBuilder LocalNav()
        {
            _item.LocalNav = true;
            return this;
        }

        public INavigationItemBuilder Local(bool value)
        {
            _item.LocalNav = value;
            return this;
        }

        public INavigationItemBuilder Permission(IPermission permission)
        {
            _item.Permissions.Add(permission);
            return this;
        }

        public INavigationItemBuilder DividerCss(string css)
        {
            _item.DividerCss = css;
            return this;
        }
        
        public INavigationItemBuilder View(string viewName, object model = null)
        {
            _item.View = new MenuItemView()
            {
                ViewName = viewName,
                Model = model
            };
            return this;
        }
        
        public INavigationItemBuilder Resource(object resource)
        {
            _item.Resource = resource;
            return this;
        }

        public override List<MenuItem> Build()
        {
            _item.Items = base.Build();
            return new List<MenuItem> { _item };
        }

        public INavigationItemBuilder Action(RouteValueDictionary values)
        {
            return values != null
                       ? Action(values["action"] as string, values["controller"] as string, values)
                       : Action(null, null, new RouteValueDictionary());
        }

        public INavigationItemBuilder Action(string actionName)
        {
            return Action(actionName, null, new RouteValueDictionary());
        }

        public INavigationItemBuilder Action(string actionName, string controllerName)
        {
            return Action(actionName, controllerName, new RouteValueDictionary());
        }

        public INavigationItemBuilder Action(string actionName, string controllerName, object values)
        {
            return Action(actionName, controllerName, new RouteValueDictionary(values));
        }

        public INavigationItemBuilder Action(string actionName, string controllerName, RouteValueDictionary values)
        {
            return Action(actionName, controllerName, null, values);
        }

        public INavigationItemBuilder Action(string actionName, string controllerName, string areaName)
        {
            return Action(actionName, controllerName, areaName, new RouteValueDictionary());
        }

        public INavigationItemBuilder Action(string actionName, string controllerName, string areaName, RouteValueDictionary values)
        {
            _item.RouteValues = new RouteValueDictionary(values);
            if (!string.IsNullOrEmpty(actionName))
                _item.RouteValues["action"] = actionName;
            if (!string.IsNullOrEmpty(controllerName))
                _item.RouteValues["controller"] = controllerName;
            if (!string.IsNullOrEmpty(areaName))
                _item.RouteValues["area"] = areaName;
            return this;
        }

    }
}
