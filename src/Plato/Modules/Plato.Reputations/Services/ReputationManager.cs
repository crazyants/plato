using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Plato.Internal.Modules.Abstractions;
using Plato.Reputations.Models;

namespace Plato.Reputations.Services
{

    public class ReputationManager<TReputation> : IReputationManager<TReputation> where TReputation : class, IReputation
    {

        private IEnumerable<TReputation> _reputations;
        private readonly IAuthorizationService _authorizationService;
        private readonly IEnumerable<IReputationProvider<TReputation>> _providers;
        private readonly ILogger<ReputationManager<TReputation>> _logger;
        private readonly ITypedModuleProvider _typedModuleProvider;
        
        public ReputationManager(
            IAuthorizationService authorizationService, 
            IEnumerable<IReputationProvider<TReputation>> providers, 
            ILogger<ReputationManager<TReputation>> logger,
            ITypedModuleProvider typedModuleProvider)
        {
            _authorizationService = authorizationService;
            _providers = providers;
            _logger = logger;
            _typedModuleProvider = typedModuleProvider;
        }

        public IEnumerable<TReputation> GetReputations()
        {
            if (_reputations == null)
            {
                var reputations = new List<TReputation>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        reputations.AddRange(provider.GetReputations());
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"An exception occurred within the reputation provider '{this.GetType()}'. Please review your reputation provider and try again. {e.Message}");
                        throw;
                    }
                }

                _reputations = reputations;
            }

            return _reputations;
        }

        public async Task<IDictionary<string, IEnumerable<TReputation>>> GetCategorizedReputationsAsync()
        {
            var output = new Dictionary<string, IEnumerable<TReputation>>();
            foreach (var provider in _providers)
            {

                var module = await _typedModuleProvider.GetModuleForDependency(provider.GetType());
                var name = module.Descriptor.Name;
                var reputations = provider.GetReputations();
                foreach (var reputation in reputations)
                {
                    var category = reputation.Category;
                    var title = String.IsNullOrWhiteSpace(category) ?
                        name :
                        category;

                    if (output.ContainsKey(title))
                    {
                        output[title] = output[title].Concat(new[] { reputation });
                    }
                    else
                    {
                        output.Add(title, new[] { reputation });
                    }
                }
            }

            return output;

        }

    }

}
