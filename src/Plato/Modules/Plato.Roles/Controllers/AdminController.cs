using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Plato.Internal.Data.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Roles;
using Plato.Internal.Navigation;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Internal.Stores.Roles;
using Plato.Roles.ViewModels;

namespace Plato.Roles.Controllers
{

    public class AdminController : Controller, IUpdateModel
    {

        #region "Constructor"

        private readonly IViewProviderManager<RolesIndexViewModel> _roleIndexViewProvider;
        private readonly IPlatoRoleStore _platoRoleStore;

        public AdminController(
            IViewProviderManager<RolesIndexViewModel> roleIndexViewProvider,
            IPlatoRoleStore platoRoleStore)
        {
            _roleIndexViewProvider = roleIndexViewProvider;
            _platoRoleStore = platoRoleStore;
        }

        #endregion

        #region "Actions"

        public async Task<ActionResult> Index(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {

            // Maintain previous route data when generating page links
            var routeData = new RouteData();
            //routeData.Values.Add("Options.Filter", options.Filter);
            routeData.Values.Add("Options.RoleName", filterOptions.RoleName);
            routeData.Values.Add("Options.Order", filterOptions.Order);

            var model = await GetPagedModel(filterOptions, pagerOptions);
            
            var result = await _roleIndexViewProvider.ProvideIndexAsync(model, this);

            return View(result);

        }

        #endregion

        #region "Private Methods"

        private async Task<RolesIndexViewModel> GetPagedModel(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            var roles = await GetRoles(filterOptions, pagerOptions);
            return new RolesIndexViewModel(
                roles,
                filterOptions,
                pagerOptions);
        }


        public async Task<IPagedResults<Role>> GetRoles(
            FilterOptions filterOptions,
            PagerOptions pagerOptions)
        {
            return await _platoRoleStore.QueryAsync()
                .Page(pagerOptions.Page, pagerOptions.PageSize)
                .Select<RoleQueryParams>(q =>
                {
                    if (filterOptions.RoleId > 0)
                    {
                        q.Id.Equals(filterOptions.RoleId);
                    }
                    if (!string.IsNullOrEmpty(filterOptions.RoleName))
                    {
                        q.RoleName.IsIn(filterOptions.RoleName);
                    }
                })
                .OrderBy("Id", OrderBy.Asc)
                .ToList<Role>();
        }

        #endregion




    }
}
