using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Shell;
using Plato.Internal.Security.Abstractions;

namespace Plato.Internal.Navigation
{
    public class NavigationManager : INavigationManager
    {

        #region "Private Variables"

        private static readonly string[] Schemes = { "http", "https", "tel", "mailto" };

        #endregion

        #region "Constructor"

        private readonly IEnumerable<INavigationProvider> _navigationProviders;
        private readonly ILogger _logger;
        private readonly IShellSettings _shellSettings;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IAuthorizationService _authorizationService;

        private IUrlHelper _urlHelper;

        public NavigationManager(
            IEnumerable<INavigationProvider> navigationProviders,
            ILogger<NavigationManager> logger,
            IShellSettings shellSettings,
            IUrlHelperFactory urlHelperFactory,
            IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            _navigationProviders = navigationProviders;
            _urlHelperFactory = urlHelperFactory;
            _shellSettings = shellSettings;
            _logger = logger;
        }

        #endregion

        #region "Implementation"

        public IEnumerable<MenuItem> BuildMenu(string name, ActionContext actionContext)
        {

            var builder = new NavigationBuilder
            {
                ActionContext = actionContext
            };

            // Processes all navigation builders to create a flat list of menu items.
            // If a navigation builder fails, it is ignored.
            foreach (var navigationProvider in _navigationProviders)
            {
                try
                {
                    navigationProvider.BuildNavigation(name, builder);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"An exception occurred while building the menu: {name}");
                }
            }

            var menuItems = builder.Build();

            // Merge all menu hierarchies into a single one
            Merge(menuItems);

            // Remove unauthorized menu items
            menuItems = Authorize(menuItems, actionContext.HttpContext.User);

            // Compute Url and RouteValues properties to Href
            menuItems = ComputeHref(menuItems, actionContext);

            // Set selected item based on matching Url
            menuItems = SetSelected(menuItems, actionContext);

            // Keep only menu items with an Href, View or DividerCss or
            // that have child items with an Href, View or DividerCss
            menuItems = Reduce(menuItems);

            // Recursive sort
            menuItems = RecursiveSort(menuItems);

            return menuItems;
        }

        #endregion

        #region "Private Methods"

        void Merge(List<MenuItem> items)
        {
            
            // Use two cursors to find all similar captions. If the same caption is represented
            // by multiple menu item, try to merge it recursively.
            for (var i = 0; i < items.Count; i++)
            {
                var source = items[i];
                var merged = false;
                for (var x = items.Count - 1; x > i; x--)
                {
                    var cursor = items[x];

                    // A match is found, add all its items to the source
                    if (String.Equals(cursor.Text.Name, source.Text.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        merged = true;
                        foreach (var child in cursor.Items)
                        {
                            source.Items.Add(child);
                        }

                        items.RemoveAt(x);

                        // If the cursor to merge is more authoritative then the source then use its values
                        if (cursor.Authority != null && source.Authority == null)
                        {

                            source.Culture = cursor.Culture;
                            source.Href = cursor.Href;
                            source.Id = cursor.Id;
                            source.LinkToFirstChild = cursor.LinkToFirstChild;
                            source.LocalNav = cursor.LocalNav;
                            source.Authority = cursor.Authority;
                            source.Order = cursor.Order;
                            source.Resource = cursor.Resource;
                            source.RouteValues = cursor.RouteValues;
                            source.Text = cursor.Text;
                            source.Url = cursor.Url;
                            source.DividerCss = cursor.DividerCss;

                            // Merge permissions
                            source.Permissions.Clear();
                            source.Permissions.AddRange(cursor.Permissions);

                            // Merge classes
                            source.Classes.Clear();
                            source.Classes.AddRange(cursor.Classes);

                        }
                    }
                }

                // If some items have been merged, apply recursively
                if (merged)
                {
                    Merge(source.Items);
                }
            }
        }
        
        List<MenuItem> ComputeHref(List<MenuItem> menuItems, ActionContext actionContext)
        {
            foreach (var menuItem in menuItems)
            {
                menuItem.Href = GetUrl(menuItem.Url, menuItem.RouteValues, actionContext);
                menuItem.Items = ComputeHref(menuItem.Items, actionContext);
            }

            return menuItems;
        }

        List<MenuItem> SetSelected(List<MenuItem> menuItems, ActionContext actionContext)
        {
            foreach (var menuItem in menuItems)
            {
                menuItem.Selected = IsHrefCurrentUrl(menuItem.Url, menuItem.RouteValues, actionContext);
                menuItem.Items = SetSelected(menuItem.Items, actionContext);
            }

            return menuItems;
        }

        bool IsHrefCurrentUrl(string menuItemUrl, RouteValueDictionary routeValueDictionary, ActionContext actionContext)
        {

       
            if (routeValueDictionary == null || routeValueDictionary.Count == 0)
            {
                if (String.IsNullOrEmpty(menuItemUrl))
                {
                    return false;
                }
                else
                {
                    if (actionContext.HttpContext != null)
                    {
                        return menuItemUrl == actionContext.HttpContext.Request.GetDisplayUrl();
                    }
                }
            }
            else
            {

                var routeValues = actionContext.RouteData.Values;
                var currentArea = routeValues["area"].ToString();
                var currentAction = routeValues["action"].ToString();
                var currentController = routeValues["controller"].ToString();
                var menuItemArea = routeValueDictionary["area"].ToString();
                var menuItemController = routeValueDictionary["controller"].ToString();
                var menuItemAction = routeValueDictionary["action"].ToString();

                var isArea = currentArea == menuItemArea;
                var isController = currentController == menuItemController;
                var isAction = currentAction == menuItemAction;

                if (isArea && isController & isAction)
                {
                    return true;
                }
            }


            return false;
        }

        string GetUrl(string menuItemUrl, RouteValueDictionary routeValueDictionary, ActionContext actionContext)
        {
            string url;
            if (routeValueDictionary == null || routeValueDictionary.Count == 0)
            {
                if (String.IsNullOrEmpty(menuItemUrl))
                {
                    return "#";
                }
                else
                {
                    url = menuItemUrl;
                }
            }
            else
            {
                if (_urlHelper == null)
                {
                    _urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);
                }

                url = _urlHelper.RouteUrl(new UrlRouteContext { Values = routeValueDictionary });
            }

            if (!string.IsNullOrEmpty(url) &&
                actionContext?.HttpContext != null &&
                !(url.StartsWith("/") ||
                Schemes.Any(scheme => url.StartsWith(scheme + ":"))))
            {
                if (url.StartsWith("~/"))
                {
                    if (!String.IsNullOrEmpty(_shellSettings.RequestedUrlPrefix))
                    {
                        url = _shellSettings.RequestedUrlPrefix + "/" + url.Substring(2);
                    }
                    else
                    {
                        url = url.Substring(2);
                    }
                }

                if (!url.StartsWith("#"))
                {
                    var appPath = actionContext.HttpContext.Request.PathBase.ToString();
                    if (appPath == "/")
                        appPath = "";
                    url = appPath + "/" + url;
                }
            }

            return url;
        }
        
        List<MenuItem> Authorize(IEnumerable<MenuItem> items, ClaimsPrincipal user)
        {
            var output = new List<MenuItem>();

            foreach (var item in items)
            {
            
                if (user == null)
                {
                    output.Add(item);
                }
                else if (!item.Permissions.Any())
                {
                    output.Add(item);
                }
                else
                {
                    foreach (var permission in item.Permissions)
                    {
                        if (_authorizationService.AuthorizeAsync(user, item.Resource, permission).Result)
                        {
                            output.Add(item);
                        }
                    }
                }

                // Process child items
                var oldItems = item.Items;

                item.Items = Authorize(item.Items, user).ToList();
            }

            return output;
        }

        List<MenuItem> Reduce(IEnumerable<MenuItem> items)
        {
            var menuItems = items as MenuItem[] ?? items.ToArray();
            var filtered = menuItems.ToList();

            foreach (var item in menuItems)
            {
                if (!HasHrefOrViewOrChildHrefOrView(item))
                {
                    filtered.Remove(item);
                }

                item.Items = Reduce(item.Items);
            }

            return filtered;
        }

        List<MenuItem> RecursiveSort(List<MenuItem> items)
        {
            if (items != null)
            {
                items = items.OrderBy(o => o.Order).ToList();
                foreach (var item in items)
                {
                    if (item.Items.Count > 0)
                    {
                        item.Items = item.Items.OrderBy(o => o.Order).ToList();
                        RecursiveSort(item.Items);
                    }
                }
            }
            
            return items?.OrderBy(o => o.Order).ToList();

        }

        bool HasHrefOrViewOrChildHrefOrView(MenuItem item)
        {
            if (item.Href != "" && item.Href != "#")
            {
                return true;
            }

            if (!String.IsNullOrEmpty(item.DividerCss))
            {
                return true;
            }

            if (item.View != null)
            {
                return true;
            }

            return item.Items.Any(HasHrefOrViewOrChildHrefOrView);
        }

        #endregion

    }
}
