using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Plato.Entities.Reactions.Models;
using Plato.Entities.Reactions.Services;
using Plato.Entities.Reactions.Stores;
using Plato.Internal.Net.Abstractions;
using Plato.WebApi.Attributes;
using Plato.WebApi.Controllers;

namespace Plato.Entities.Reactions.Controllers
{

    //public class EntityController : BaseWebApiController
    //{
    //    private readonly IClientIpAddress _clientIpAddress;
    //    private readonly IEntityReactionsManager<EntityReaction> _entityReactionManager;
    //    private readonly IEntityReactionsStore<EntityReaction> _entityReactionsStore;

    //    public EntityController(
    //        IEntityReactionsStore<EntityReaction> entityReactionsStore,
    //        IEntityReactionsManager<EntityReaction> entityReactionManager, 
    //        IClientIpAddress clientIpAddress)
    //    {
    //        _entityReactionsStore = entityReactionsStore;
    //        _entityReactionManager = entityReactionManager;
    //        _clientIpAddress = clientIpAddress;
    //    }

    //    [HttpGet, ResponseCache(NoStore = true)]
    //    public async Task<IActionResult> Get(int id)
    //    {
    //        var data = await _entityReactionsStore.GetByIdAsync(id);
    //        if (data != null)
    //        {
    //            return base.Result(data);
    //        }
    //        return base.NotFound();
    //    }
        

    //    [HttpPost, ValidateClientAntiForgeryToken, ResponseCache(NoStore = true)]
    //    public async Task<IActionResult> Post([FromBody] EntityReaction model)
    //    {
            
    //        // We need a user to subscribe to the entity
    //        var user = await base.GetAuthenticatedUserAsync();
    //        if (user == null)
    //        {
    //            return base.UnauthorizedException();
    //        }

    //        model.CreatedUserId = user.Id;
    //        model.CreatedDate = DateTimeOffset.UtcNow;
    //        model.IpV4Address = _clientIpAddress.GetIpV4Address();
    //        model.IpV6Address = _clientIpAddress.GetIpV6Address();
    //        if (Request.Headers.ContainsKey("User-Agent"))
    //        {
    //            model.UserAgent = Request.Headers["User-Agent"].ToString();
    //        }
            
    //        // Add and return result
    //        var result = await _entityReactionManager.CreateAsync(model);
    //        if (result.Succeeded)
    //        {
    //            return base.Created(result);
    //        }

    //        // We should not reach here
    //        return base.InternalServerError();

    //    }

    //    [HttpPut, ValidateClientAntiForgeryToken, ResponseCache(NoStore = true)]
    //    public async Task<IActionResult> Put(EntityReaction model)
    //    {
  
    //        var user = await base.GetAuthenticatedUserAsync();
    //        if (user == null)
    //        {
    //            return base.UnauthorizedException();
    //        }

    //        var result = await _entityReactionManager.UpdateAsync(model);
    //        if (result.Succeeded)
    //        {
    //            return base.Created(result);
    //        }

    //        // We should not reach here
    //        return base.InternalServerError();

    //    }

    //    [HttpDelete, ValidateClientAntiForgeryToken, ResponseCache(NoStore = true)]
    //    public async Task<IActionResult> Delete([FromBody] EntityReaction model)
    //    {

    //        var user = await base.GetAuthenticatedUserAsync();
    //        if (user == null)
    //        {
    //            return base.UnauthorizedException();
    //        }
            
    //        var existingReaction = await _entityReactionsStore.GetByIdAsync(model.Id);
    //        if (existingReaction != null)
    //        {
    //            var success = await _entityReactionsStore.DeleteAsync(existingReaction);
    //            if (success)
    //            {
    //                return base.Result(existingReaction);
    //            }
    //        }

    //        // We should not reach here
    //        return base.InternalServerError();

    //    }
        
    //}

}
