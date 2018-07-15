using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Plato.Categories.Models;
using Plato.Categories.Services;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Layout.ModelBinding;
using Plato.Internal.Layout.ViewProviders;
using Plato.Internal.Models.Shell;
using Plato.Internal.Models.Users;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Roles.ViewModels;

namespace Plato.Roles.ViewProviders
{
    public class CategoryViewProvider : BaseViewProvider<Category>
    {

        private const string HtmlName = "UserRoles";

        private readonly ICategoryManager<Category> _categoryManager;
        private readonly IPlatoRoleStore _platoRoleStore;
        private readonly IContextFacade _contextFacade;


        private readonly HttpRequest _request;

        public CategoryViewProvider(
            IPlatoRoleStore platoRoleStore,
            IHttpContextAccessor httpContextAccessor,
            ICategoryManager<Category> categoryManager,
            IContextFacade contextFacade)
        {
          
            _platoRoleStore = platoRoleStore;
            _categoryManager = categoryManager;
            _contextFacade = contextFacade;
            _request = httpContextAccessor.HttpContext.Request;
        }


        public override Task<IViewProviderResult> BuildDisplayAsync(Category user, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override Task<IViewProviderResult> BuildIndexAsync(Category user, IUpdateModel updater)
        {
            return Task.FromResult(default(IViewProviderResult));
        }

        public override async Task<IViewProviderResult> BuildEditAsync(Category user, IUpdateModel updater)
        {

            var selectedRoles = await _platoRoleStore.GetRoleNamesByUserIdAsync(user.Id);

            return Views(
                View<EditUserRolesViewModel>("Category.Roles.Edit.Content", model =>
                {
                    model.SelectedRoles = selectedRoles;
                    model.HtmlName = HtmlName;
                    return model;
                }).Order(2)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Category viewModel, IUpdateModel updater)
        {

            // Get available role names
            var roleNames = await _platoRoleStore.GetRoleNamesAsync();

            // Convert to list to prevent multiple enumerations below
            var roleNamesList = roleNames.ToList();

            // Build selected roles
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

            // Update model
            var model = new EditUserRolesViewModel();
            model.SelectedRoles = rolesToAdd;

            if (!await updater.TryUpdateModelAsync(model))
            {
                return await BuildEditAsync(viewModel, updater);
            }

            if (updater.ModelState.IsValid)
            {

                var featureId = 0;
                var feature = await _contextFacade.GetFeatureByAreaAsync();
                if (feature != null)
                {
                    featureId = feature.Id;
                }

                viewModel.FeatureId = featureId;

                var result = viewModel.Id == 0
                    ? await _categoryManager.CreateAsync(viewModel)
                    : await _categoryManager.UpdateAsync(viewModel);
                
                if (result.Succeeded)
                {

                    // Remove roles in two steps to prevent an iteration on a modified collection
                    var rolesToRemove = new List<string>();
                    foreach (var role in await _categoryManager.GetRolesAsync(result.Response))
                    {
                        if (!rolesToAdd.Contains(role))
                        {
                            rolesToRemove.Add(role);
                        }
                    }

                    foreach (var role in rolesToRemove)
                    {
                        await _categoryManager.RemoveFromRoleAsync(result.Response, role);
                    }

                    // Add new roles
                    foreach (var role in rolesToAdd)
                    {
                        if (!await _categoryManager.IsInRoleAsync(result.Response, role))
                        {
                            await _categoryManager.AddToRoleAsync(result.Response, role);
                        }
                    }

                    foreach (var error in result.Errors)
                    {
                        updater.ModelState.AddModelError(string.Empty, error.Description);
                    }
                    
                    return await BuildEditAsync(result.Response, updater);

                }

            }

            return await BuildEditAsync(viewModel, updater);

        }

        async Task<ShellModule> GetcurrentFeature()
        {
            var feature = await _contextFacade.GetFeatureByAreaAsync();
            if (feature == null)
            {
                throw new Exception("No feature could be found");
            }
            return feature;
        }


    }
}
