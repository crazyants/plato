using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Categories.Models;
using Plato.Categories.Stores;
using Plato.WebApi.Controllers;

namespace Plato.Categories.Controllers
{

    public class CategoriesController : BaseWebApiController
    {

        private readonly ICategoryStore<CategoryBase> _categoryStore;

        public CategoriesController(
            ICategoryStore<CategoryBase> categoryStore)
        {
            _categoryStore = categoryStore;
        }

        [HttpGet]
        [ResponseCache(NoStore = true)]
        public async Task<IActionResult> Get(int featureId = 0)
        {
            var categories = await _categoryStore.GetByFeatureIdAsync(featureId);
            if (categories != null)
            {
                return base.Result(categories);
            }
            return base.NoResults();
        }
        
        [HttpPost, ResponseCache(NoStore = true)]
        public Task<IActionResult> Post([FromBody] CategoryBase categoryBase)
        {
     
            throw new NotImplementedException();

            //// We need a user to subscribe to the entity
            //var user = await base.GetAuthenticatedUserAsync();
            //if (user == null)
            //{
            //    return base.UnauthorizedException();
            //}

            //// Is the user already following the entity?
            //var existingFollow = await _entityFollowStore.SelectEntityFollowByUserIdAndEntityId(user.Id, follow.EntityId);
            //if (existingFollow != null)
            //{
            //    return base.Result(HttpStatusCode.OK,
            //        $"Authenticated user already following entity with id '{follow.EntityId}'");
            //}

            //// Build a new subscription
            //var followToAdd = new EntityFollow()
            //{
            //    EntityId = follow.EntityId,
            //    UserId = user.Id,
            //    CreatedDate = DateTime.UtcNow
            //};

            //// Add and return result
            //var result = await _entityFollowStore.CreateAsync(followToAdd);
            //if (result != null)
            //{
            //    return base.Result(result);
            //}

            //// We should not reach here
            //return base.InternalServerError();

        }

        [HttpPut]
        [ResponseCache(NoStore = true)]
        public Task<IActionResult> Put(CategoryBase categoryBase)
        {
            throw new NotImplementedException();
        }
        
        [HttpDelete]
        [ResponseCache(NoStore = true)]
        public Task<IActionResult> Delete([FromBody] CategoryBase categoryBase)
        {

            throw new NotImplementedException();

            //var user = await base.GetAuthenticatedUserAsync();
            //if (user == null)
            //{
            //    return base.UnauthorizedException();
            //}
            
            //var existingFollow = await _entityFollowStore.SelectEntityFollowByUserIdAndEntityId(user.Id, follow.EntityId);
            //if (existingFollow != null)
            //{
            //    var success = await _entityFollowStore.DeleteAsync(existingFollow);
            //    if (success)
            //    {
            //        return base.Result(existingFollow);
            //    }

            //}

            //// We should not reach here
            //return base.InternalServerError();

        }

    }
}
