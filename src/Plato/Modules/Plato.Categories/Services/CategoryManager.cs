using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;

namespace Plato.Categories.Services
{

    public class CategoryManager<TCategory> : ICategoryManager<TCategory> where TCategory : class, ICategory
    {

        private readonly ICategoryStore<TCategory> _categoryStore;
        private readonly ICategoryRoleStore<CategoryRole> _categoryRoleStore;
        private readonly IContextFacade _contextFacade;
        private readonly IPlatoRoleStore _roleStore;

        public CategoryManager(
            ICategoryStore<TCategory> categoryStore,
            ICategoryRoleStore<CategoryRole> categoryRoleStore,
            IPlatoRoleStore roleStore, 
            IContextFacade contextFacade)
        {
            _categoryStore = categoryStore;
            _categoryRoleStore = categoryRoleStore;
            _roleStore = roleStore;
            _contextFacade = contextFacade;
        }

        #region "Implementation"

        public async Task<IActivityResult<TCategory>> CreateAsync(TCategory model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.FeatureId <= 0)
            {
                throw new ArgumentNullException(nameof(model.FeatureId));
            }
            
            if (model.Id > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }
            
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            if (model.CreatedUserId == 0)
            {
                model.CreatedUserId = user?.Id ?? 0;
            }
            
            model.CreatedDate = DateTime.UtcNow;

            var result = new ActivityResult<TCategory>();

            var category = await _categoryStore.CreateAsync(model);
            if (category != null)
            {
                // Return success
                return result.Success(category);
            }

            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to create the category"));


        }

        public async Task<IActivityResult<TCategory>> UpdateAsync(TCategory model)
        {
            
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.FeatureId <= 0)
            {
                throw new ArgumentNullException(nameof(model));
            }
            
            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }
            
            var user = await _contextFacade.GetAuthenticatedUserAsync();

            model.ModifiedUserId = user?.Id ?? 0;
            model.ModifiedDate = DateTime.UtcNow;

            var result = new ActivityResult<TCategory>();

            var category = await _categoryStore.UpdateAsync(model);
            if (category != null)
            {
                // Return success
                return result.Success(category);
            }

            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to update the category"));
            
        }

        public async Task<IActivityResult<TCategory>> DeleteAsync(TCategory model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var result = new ActivityResult<TCategory>();
            
            var success = await _categoryStore.DeleteAsync(model);
            if (success)
            {
                // Return success
                return result.Success();
            }
            
            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to delete the category"));


        }

        public async Task<IActivityResult<TCategory>> AddToRoleAsync(TCategory model, string roleName)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }
            
            var result = new ActivityResult<TCategory>();

            var role = await _roleStore.GetByNameAsync(roleName);
            if (role == null)
            {
                return result.Failed(new ActivityError($"A role with the name {roleName} could not be found"));
            }

            var user = await _contextFacade.GetAuthenticatedUserAsync();
        
            var categoryRole = new CategoryRole()
            {
                CategoryId = model.Id,
                RoleId = role.Id,
                CreatedUserId = user?.Id ?? 0
            };

            var updatedCategoryRole = await _categoryRoleStore.CreateAsync(categoryRole);
            if (updatedCategoryRole != null)
            {
                return result.Success();
            }

            return result.Failed(new ActivityError($"An unknown error occurred whilst attempting to add role '{role.Name}' for category '{model.Name}'"));

        }

        public async Task<IActivityResult<TCategory>> RemoveFromRoleAsync(TCategory model, string roleName)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }
            
            // Our result
            var result = new ActivityResult<TCategory>();
            
            // Ensure the role exists
            var role = await _roleStore.GetByNameAsync(roleName);
            if (role == null)
            {
                return result.Failed(new ActivityError($"A role with the name {roleName} could not be found"));
            }
            
            // Attempt to delete the role relationship
            var success = await _categoryRoleStore.DeleteByRoleIdAndCategoryIdAsync(role.Id, model.Id);
            if (success)
            {
                return result.Success();
            }
            
            return result.Failed(new ActivityError($"An unknown error occurred whilst attempting to remove role '{role.Name}' for category '{model.Name}'"));

        }

        public async Task<bool> IsInRoleAsync(TCategory model, string roleName)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (String.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException(nameof(roleName));
            }

            var result = new ActivityResult<TCategory>();

            var role = await _roleStore.GetByNameAsync(roleName);
            if (role == null)
            {
                return false;
            }
            
            var roles = await _categoryRoleStore.GetByCategoryIdAsync(model.Id);
            if (roles == null)
            {
                return false;
            }

            foreach (var localRole in roles)
            {
                if (localRole.Id == role.Id)
                {
                    return true;
                }
            }

            return false;

        }

        public async Task<IEnumerable<string>> GetRolesAsync(TCategory model)
        {

            var roles = await _categoryRoleStore.GetByCategoryIdAsync(model.Id);
            if (roles != null)
            {
                return roles.Select(s => s.RoleName).ToArray();
            }

            return new string[] {};

        }

        #endregion

    }

}
