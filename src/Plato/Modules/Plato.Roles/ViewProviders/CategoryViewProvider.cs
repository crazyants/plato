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

        public override async Task<IViewProviderResult> BuildEditAsync(Category category, IUpdateModel updater)
        {

            var selectedRoles = await _categoryManager.GetRolesAsync(category);

            return Views(
                View<EditUserRolesViewModel>("Category.Roles.Edit.Content", model =>
                {
                    model.SelectedRoles = selectedRoles;
                    model.HtmlName = HtmlName;
                    return model;
                }).Order(2)
            );

        }

        public override async Task<IViewProviderResult> BuildUpdateAsync(Category category, IUpdateModel updater)
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
                return await BuildEditAsync(category, updater);
            }

            if (updater.ModelState.IsValid)
            {

                //var featureId = 0;
                //var feature = await _contextFacade.GetFeatureByAreaAsync();
                //if (feature != null)
                //{
                //    featureId = feature.Id;
                //}

                //category.FeatureId = featureId;

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

                //var result = await _categoryManager.UpdateAsync(new Category()
                //{
                //    Id = model.Id,
                //    FeatureId = model.FeatureId,
                //    Name = model.Name,
                //    Description = model.Description,
                //    ForeColor = model.ForeColor,
                //    BackColor = model.BackColor,
                //    IconCss = model.IconPrefix + model.IconCss
                //});

                //if (result.Succeeded)
                //{

                //    return await BuildEditAsync(result.Response, updater);

                //}
                //else
                //{
                //    foreach (var error in result.Errors)
                //    {
                //        updater.ModelState.AddModelError(string.Empty, error.Description);
                //    }

                //}

            }

            return await BuildEditAsync(category, updater);

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
