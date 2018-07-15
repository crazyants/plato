using System;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;

namespace Plato.Categories.Services
{

    public class CategoryManager<TCategory> : ICategoryManager<TCategory> where TCategory : class, ICategory
    {

        private readonly ICategoryStore<TCategory> _categoryStore;
        private readonly ICategoryRoleStore<CategoryRole> _categoryRoleStore;
        private readonly IPlatoRoleStore _roleStore;

        public CategoryManager(
            ICategoryStore<TCategory> categoryStore,
            ICategoryRoleStore<CategoryRole> categoryRoleStore,
            IPlatoRoleStore roleStore)
        {
            _categoryStore = categoryStore;
            _categoryRoleStore = categoryRoleStore;
            _roleStore = roleStore;
        }

        #region "Implementation"

        public async Task<IActivityResult<TCategory>> CreateAsync(TCategory model)
        {

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var result = new ActivityResult<TCategory>();

            var category = await _categoryStore.CreateAsync(model);
            if (category != null)
            {

                // Return success
                return result.Success(category);
            }

            return result.Failed(new EntityError("An unknown error occurred whilst attempting to create the category"));


        }

        public async Task<IActivityResult<TCategory>> UpdateAsync(TCategory model)
        {


            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }


            var result = new ActivityResult<TCategory>();

            var category = await _categoryStore.UpdateAsync(model);
            if (category != null)
            {
                // Return success
                return result.Success(category);
            }

            return result.Failed(new EntityError("An unknown error occurred whilst attempting to update the category"));


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
            
            return result.Failed(new EntityError("An unknown error occurred whilst attempting to delete the category"));


        }

        public async Task<IActivityResult<TCategory>> AddToRolesAsync(TCategory model, string roleName)
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

            var role = await _roleStore.GetByName(roleName);
            if (role == null)
            {
                return result.Failed(new EntityError($"A role with the name {roleName} could not be found"));
            }

            var categoryRole = new CategoryRole()
            {
                CategoryId = model.Id,
                RoleId = role.Id
            };

            var newCategoryRole = _categoryRoleStore.CreateAsync(categoryRole);
            if (newCategoryRole != null)
            {
                return result.Success(newCategoryRole);
            }

            return result.Failed(new EntityError($"An unknown error occurred whilst attempting to add role '{role.Name}' for category '{model.Name}'"));

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
            
            var result = new ActivityResult<TCategory>();
            
            var role = await _roleStore.GetByName(roleName);
            if (role == null)
            {
                return result.Failed(new EntityError($"A role with the name {roleName} could not be found"));
            }
            
            var success = await _categoryRoleStore.DeleteByRoleIdAndCategoryIdAsync(role.Id, model.Id);
            if (success)
            {
                return result.Success();
            }
            
            return result.Failed(new EntityError($"An unknown error occurred whilst attempting to remove role '{role.Name}' for category '{model.Name}'"));

        }

        public async Task<IActivityResult<TCategory>> IsInRoleAsync(TCategory model, string roleName)
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

            var role = await _roleStore.GetByName(roleName);
            if (role == null)
            {
                return result.Failed(new EntityError($"A role with the name {roleName} could not be found"));
            }
            
            var roles = await _categoryRoleStore.GetByCategoryIdAsync(model.Id);

            foreach (var localRole in roles)
            {
                if (localRole.Id == role.Id)
                {
                    return result.Success(true);
                }
            }

            return result.Failed();

        }

        #endregion

    }

}
