using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Plato.Internal.Modules.Abstractions;
using Plato.Reactions.Models;

namespace Plato.Reactions.Services
{
    
    public class ReactionsManager<TReaction> : IReactionsManager<TReaction> where TReaction : class, IReaction
    {

        private IEnumerable<TReaction> _reactions;

        private readonly IAuthorizationService _authorizationService;
        private readonly IEnumerable<IReactionsProvider<TReaction>> _providers;
        private readonly ILogger<ReactionsManager<TReaction>> _logger;
        private readonly ITypedModuleProvider _typedModuleProvider;

        public ReactionsManager(
            IAuthorizationService authorizationService,
            IEnumerable<IReactionsProvider<TReaction>> providers, 
            ILogger<ReactionsManager<TReaction>> logger,
            ITypedModuleProvider typedModuleProvider)
        {
            _authorizationService = authorizationService;
            _providers = providers;
            _logger = logger;
            _typedModuleProvider = typedModuleProvider;
        }
        
        public IEnumerable<TReaction> GetReactions()
        {
            if (_reactions == null)
            {
                var reactions = new List<TReaction>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        reactions.AddRange(provider.GetReactions());
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"An exception occurred within the reaction provider '{this.GetType()}'. Please review your reaction provider and try again. {e.Message}");
                        throw;
                    }
                }

                _reactions = reactions;
            }

            return _reactions;
        }

        public async Task<IDictionary<string, IEnumerable<TReaction>>> GetCategorizedReactionsAsync()
        {
            var output = new Dictionary<string, IEnumerable<TReaction>>();
            foreach (var provider in _providers)
            {

                var module = await _typedModuleProvider.GetModuleForDependency(provider.GetType());
                var name = module.Descriptor.Name;
                var badges = provider.GetReactions();
                foreach (var badge in badges)
                {
                    var category = badge.Category;
                    var title = String.IsNullOrWhiteSpace(category) ?
                        name :
                        category;

                    if (output.ContainsKey(title))
                    {
                        output[title] = output[title].Concat(new[] { badge });
                    }
                    else
                    {
                        output.Add(title, new[] { badge });
                    }
                }
            }

            return output;

        }

    }

}
