using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Plato.Entities.Models;
using Plato.Entities.Repositories;
using Plato.Entities.Services;
using Plato.Internal.Layout;
using Plato.Internal.Layout.ActionFilters;
using Plato.Internal.Models.Metrics;
using Plato.Internal.Security.Abstractions;

namespace Plato.Entities.ActionFilters
{

    public class HomeMenuContextualizeFilter : IModularActionFilter
    {

        private readonly IAggregatedFeatureEntitiesService _aggregatedFeatureEntitiesService;
        private readonly IAggregatedEntityRepository _aggregatedEntityRepository;
        private readonly IAuthorizationService _authorizationService;

        public HomeMenuContextualizeFilter(
            IAggregatedEntityRepository aggregatedEntityRepository, 
            IAggregatedFeatureEntitiesService aggregatedFeatureEntitiesService,
            IAuthorizationService authorizationService)
        {
            _aggregatedEntityRepository = aggregatedEntityRepository;
            _aggregatedFeatureEntitiesService = aggregatedFeatureEntitiesService;
            _authorizationService = authorizationService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            return;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        public async Task OnActionExecutingAsync(ResultExecutingContext context)
        {

            // The controller action didn't return a view result so no need to continue execution
            if (!(context.Result is ViewResult result))
            {
                return;
            }
            
            // Check early to ensure we are working with a LayoutViewModel
            if (!(result.Model is LayoutViewModel model))
            {
                return;
            }

            // Plato.Core?
            var isArea = context.RouteData.Values["area"].ToString()
                .Equals("Plato.Core", StringComparison.OrdinalIgnoreCase);

            if (!isArea)
            {
                return;
            }
            
            // Home controller?
            var isController = context.RouteData.Values["controller"].ToString()
                .Equals("Home", StringComparison.OrdinalIgnoreCase);

            if (!isController)
            {
                return;
            }

            // Index action?
            var isAction = context.RouteData.Values["action"].ToString()
                .Equals("Index", StringComparison.OrdinalIgnoreCase);
            
            if (!isAction)
            {
                return;
            }
            
         
            
            // We are on the homepage, register metrics on context
            context.HttpContext.Items[typeof(FeatureEntityMetrics)] = new FeatureEntityMetrics()
            {
                AggregatedResults = await _aggregatedFeatureEntitiesService
                    .ConfigureQuery(async q =>
                    {

                        // Hide private?
                        if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User,
                            Permissions.ViewPrivateEntities))
                        {
                            q.HidePrivate.True();
                        }

                        // Hide hidden?
                        if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User,
                            Permissions.ViewHiddenEntities))
                        {
                            q.HideHidden.True();
                        }

                        // Hide spam?
                        if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User,
                            Permissions.ViewSpamEntities))
                        {
                            q.HideSpam.True();
                        }

                        // Hide deleted?
                        if (!await _authorizationService.AuthorizeAsync(context.HttpContext.User,
                            Permissions.ViewDeletedEntities))
                        {
                            q.HideDeleted.True();
                        }

                    })
                    .GetResultsAsync(null)
            };

        }

        public Task OnActionExecutedAsync(ResultExecutingContext context)
        {
            return Task.CompletedTask;
        }
    }

}
