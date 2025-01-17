﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Plato.Internal.Badges.Abstractions;
using Plato.Internal.Models.Badges;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Internal.Badges
{

    public class BadgesManager<TBadge> : IBadgesManager<TBadge> where TBadge : class, IBadge
    {

        private IEnumerable<TBadge> _badges;
        
        private readonly IEnumerable<IBadgesProvider<TBadge>> _providers;
        private readonly ITypedModuleProvider _typedModuleProvider;
        private readonly ILogger<BadgesManager<TBadge>> _logger;

        public BadgesManager(
            IEnumerable<IBadgesProvider<TBadge>> providers,
            ILogger<BadgesManager<TBadge>> logger,
            ITypedModuleProvider typedModuleProvider)
        {
            _typedModuleProvider = typedModuleProvider;
            _providers = providers;            
            _logger = logger;
        }

        public IEnumerable<TBadge> GetBadges()
        {
            if (_badges == null)
            {
                var badges = new List<TBadge>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        badges.AddRange(provider.GetBadges());
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"An exception occurred within the badge provider '{provider.GetType()}'. Please review your badge provider and try again. {e.Message}");
                        throw;
                    }
                }

                _badges = badges;
            }

            return _badges;
        }

        public async Task<IDictionary<string, IEnumerable<TBadge>>> GetCategorizedBadgesAsync()
        {

            var output = new Dictionary<string, IEnumerable<TBadge>>();
            foreach (var provider in _providers)
            {

                var module = await _typedModuleProvider.GetModuleForDependency(provider.GetType());
                var name = module.Descriptor.Name;
                var badges = provider.GetBadges();
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
