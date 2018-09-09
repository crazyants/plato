using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.Internal.Abstractions;
using Plato.Internal.Hosting.Abstractions;
using Plato.Internal.Messaging.Abstractions;
using Plato.Internal.Shell.Abstractions;
using Plato.Internal.Stores.Abstractions.Roles;
using Plato.Internal.Text.Abstractions;

namespace Plato.Categories.Services
{

    public class CategoryManager<TCategory> : ICategoryManager<TCategory> where TCategory : class, ICategory
    {
        
        
        private readonly ICategoryRoleStore<CategoryRole> _categoryRoleStore;
        private readonly ICategoryDataStore<CategoryData> _categoryDataStore;
        private readonly ICategoryStore<TCategory> _categoryStore;
        private readonly IContextFacade _contextFacade;
        private readonly IAliasCreator _aliasCreator;
        private readonly IPlatoRoleStore _roleStore;
        private readonly IBroker _broker;

        public CategoryManager(
            ICategoryStore<TCategory> categoryStore,
            ICategoryRoleStore<CategoryRole> categoryRoleStore,
            ICategoryDataStore<CategoryData> categoryDataStore,
            IContextFacade contextFacade,
            IAliasCreator aliasCreator,
            IPlatoRoleStore roleStore,
            IBroker broker)
        {
            _categoryStore = categoryStore;
            _categoryRoleStore = categoryRoleStore;
            _roleStore = roleStore;
            _contextFacade = contextFacade;
            _categoryDataStore = categoryDataStore;
            _broker = broker;
            _aliasCreator = aliasCreator;
        }

        #region "Implementation"

        public async Task<IActivityResult<TCategory>> CreateAsync(TCategory model)
        {

            // Validate
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
            
            // Configure model

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            if (model.CreatedUserId == 0)
            {
                model.CreatedUserId = user?.Id ?? 0;
            }
            
            model.CreatedDate = DateTime.UtcNow;
            model.Alias = await ParseAlias(model.Name);
          
            // Publish CategoryCreating event
            await _broker.Pub<TCategory>(this, new MessageOptions()
            {
                Key = "CategoryCreating"
            }, model);

            var result = new ActivityResult<TCategory>();

            var category = await _categoryStore.CreateAsync(model);
            if (category != null)
            {
                // Publish CategoryCreated event
                await _broker.Pub<TCategory>(this, new MessageOptions()
                {
                    Key = "CategoryCreated"
                }, category);
                // Return success
                return result.Success(category);
            }

            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to create the category"));
            
        }

        public async Task<IActivityResult<TCategory>> UpdateAsync(TCategory model)
        {
            
            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.FeatureId <= 0)
            {
                throw new ArgumentNullException(nameof(model.FeatureId));
            }
            
            if (model.Id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(model.Id));
            }

            if (String.IsNullOrWhiteSpace(model.Name))
            {
                throw new ArgumentNullException(nameof(model.Name));
            }
            
            // Configure model

            var user = await _contextFacade.GetAuthenticatedUserAsync();
            model.ModifiedUserId = user?.Id ?? 0;
            model.ModifiedDate = DateTime.UtcNow;
            model.Alias = await ParseAlias(model.Name);
            
            // Publish CategoryUpdating event
            await _broker.Pub<TCategory>(this, new MessageOptions()
            {
                Key = "CategoryUpdating"
            }, model);

            var result = new ActivityResult<TCategory>();

            var category = await _categoryStore.UpdateAsync(model);
            if (category != null)
            {
                // Publish CategoryUpdated event
                await _broker.Pub<TCategory>(this, new MessageOptions()
                {
                    Key = "CategoryUpdated"
                }, category);
                // Return success
                return result.Success(category);
            }

            return result.Failed(new ActivityError("An unknown error occurred whilst attempting to update the category"));
            
        }

        public async Task<IActivityResult<TCategory>> DeleteAsync(TCategory model)
        {

            // Validate
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Publish CategoryDeleting event
            await _broker.Pub<TCategory>(this, new MessageOptions()
            {
                Key = "CategoryDeleting"
            }, model);

            var result = new ActivityResult<TCategory>();
            if (await _categoryStore.DeleteAsync(model))
            {
                // Delete category roles
                await _categoryRoleStore.DeleteByCategoryIdAsync(model.Id);

                // Delete category data
                var data = await _categoryDataStore.GetByCategoryIdAsync(model.Id);
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        await _categoryDataStore.DeleteAsync(item);
                    }
                    
                }

                // Publish CategoryDeleted event
                await _broker.Pub<TCategory>(this, new MessageOptions()
                {
                    Key = "CategoryDeleted"
                }, model);

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

            // Ensure the role exists
            var existingRole = await _roleStore.GetByNameAsync(roleName);
            if (existingRole == null)
            {
                return result.Failed(new ActivityError($"A role with the name {roleName} could not be found"));
            }

            // Ensure supplied role name is not already associated with the category
            var roles = await _categoryRoleStore.GetByCategoryIdAsync(model.Id);
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    if (role.RoleName.Equals(roleName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return result.Failed(new ActivityError($"A role with the name '{roleName}' is already associated with the category '{model.Name}'"));
                    }
                }
            }
            
            var user = await _contextFacade.GetAuthenticatedUserAsync();
        
            var categoryRole = new CategoryRole()
            {
                CategoryId = model.Id,
                RoleId = existingRole.Id,
                CreatedUserId = user?.Id ?? 0
            };

            var updatedCategoryRole = await _categoryRoleStore.CreateAsync(categoryRole);
            if (updatedCategoryRole != null)
            {
                return result.Success();
            }

            return result.Failed(new ActivityError($"An unknown error occurred whilst attempting to add role '{existingRole.Name}' for category '{model.Name}'"));

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

        #region "Private Methods"

        private async Task<string> ParseAlias(string input)
        {

            foreach (var handler in await _broker.Pub<string>(this, new MessageOptions()
            {
                Key = "ParseCategoryAlias"
            }, input))
            {
                return await handler.Invoke(new Message<string>(input, this));
            }

            // No subscription found, use default alias creator
            return _aliasCreator.Create(input);

        }

        #endregion

    }

}
