using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Models.Reputations;
using Plato.Internal.Modules.Abstractions;
using Plato.Internal.Reputations.Abstractions;

namespace Plato.Internal.Reputations
{

    public class ReputationsManager<TReputation> : IReputationsManager<TReputation> where TReputation : class, IReputation
    {
        
        private readonly IEnumerable<IReputationsProvider<TReputation>> _providers;
        private readonly ILogger<ReputationsManager<TReputation>> _logger;
        private readonly ITypedModuleProvider _typedModuleProvider;
        private IEnumerable<TReputation> _reputations;

        public ReputationsManager(
            IEnumerable<IReputationsProvider<TReputation>> providers, 
            ILogger<ReputationsManager<TReputation>> logger,
            ITypedModuleProvider typedModuleProvider)
        {
            _typedModuleProvider = typedModuleProvider;
            _providers = providers;
            _logger = logger;
        }

        public async Task<IEnumerable<TReputation>> GetReputationsAsync()
        {
            if (_reputations == null)
            {
                var reputations = new List<TReputation>();
                foreach (var provider in _providers)
                {
                    var module = await _typedModuleProvider.GetModuleForDependency(provider.GetType());
                    try
                    {
                        foreach (var reputation in provider.GetReputations())
                        {
                            reputation.ModuleId = module?.Descriptor?.Id;
                            reputations.Add(reputation);
                        }
                  
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"An exception occurred within the reputation provider '{provider.GetType()}'. Please review your reputation provider and try again. {e.Message}");
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
