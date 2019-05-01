using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Discuss.Categories.Models;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Roles.ViewModels;

namespace Plato.Discuss.Categories.ViewProviders
{

    public class ChannelRolesViewProvider : BaseViewProvider<ChannelAdmin>
    {

        private const string HtmlName = "ChannelRoles";

        private readonly ICategoryManager<CategoryBase> _categoryManager;
        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly IContextFacade _contextFacade;


        private readonly HttpRequest _request;

        public ChannelRolesViewProvider(
            IPlatoRoleStore platoRoleStore,
            IHttpContextAccessor httpContextAccessor,
            ICategoryManager<CategoryBase> categoryManager,
            IContextFacade contextFacade)
        {
          
            _platoRoleStore = platoRoleStore;
            _categoryManager = categoryManager;
            _contextFacade = contextFacade;
            _request = httpContextAccessor.HttpContext.Request;
        }


        public override Task<IViewProviderResult> BuildDisplayAsync(ChannelAdmin category, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(ChannelAdmin category, IViewProviderContext updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(ChannelAdmin category, IViewProviderContext updater)
        {

            var selectedRoles = await _categoryManager.GetRolesAsync(category);

            return Views(
                View<EditUserRolesViewModel>("Admin.Edit.ChannelRoles", model =>
                {
                    model.SelectedRoles = selectedRoles;
                    model.HtmlName = HtmlName;
                    return model;
                }).Order(2)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(ChannelAdmin category,
            IViewProviderContext context)
        {
            
            // Get posted role names
            var rolesToAdd = GetRolesToAdd();
            
            // Update model
            var model = new EditUserRolesViewModel
            {
                SelectedRoles = rolesToAdd
            };

            if (!await context.Updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(category, context);
            }

            if (context.Updater.ModelState.IsValid)
            {

                // Remove roles in two steps to prevent an iteration on a modified collection
                var rolesToRemove = new List<string>();
                foreach (var role in await _categoryManager.GetRolesAsync(category))
                {
                    if (!rolesToAdd.Contains(role))
                    {
                        rolesToRemove.Add(role);
                    }
                }

                foreach (var role in rolesToRemove)
                {
                    await _categoryManager.RemoveFromRoleAsync(category, role);
                }

                // Add new roles
                foreach (var role in rolesToAdd)
                {
                    if (!await _categoryManager.IsInRoleAsync(category, role))
                    {
                        await _categoryManager.AddToRoleAsync(category, role);
                    }
                }

            }

            return await BuildEditAsync(category, context);

        }

        List<string> GetRolesToAdd()
        {
            var rolesToAdd = new List<string>();
            foreach (var key in _request.Form.Keys)
            {
                if (key.StartsWith(HtmlName))
                {
                    var values = _request.Form[key];
                    foreach (var value in values)
                    {
                        if (!rolesToAdd.Contains(value))
                        {
                            rolesToAdd.Add(value);
                        }
                    }
                }
            }

            return rolesToAdd;

        }

    }

}
