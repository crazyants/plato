using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Plato.Badges.Models;
using Plato.Internal.Modules.Abstractions;

namespace Plato.Badges.Services
{
    public class BadgesManager<TBadge> : IBadgesManager<TBadge> where TBadge : class, IBadge
    {

        private IEnumerable<TBadge> _permissions;

        private readonly IAuthorizationService _authorizationService;
        private readonly IEnumerable<IBadgeProvider<TBadge>> _providers;
        private readonly ILogger<BadgesManager<TBadge>> _logger;
        private readonly ITypedModuleProvider _typedModuleProvider;

        public BadgesManager(
            IEnumerable<IBadgeProvider<TBadge>> providers,
            ILogger<BadgesManager<TBadge>> logger,
            ITypedModuleProvider typedModuleProvider,
            IAuthorizationService authorizationService)
        {
            _providers = providers;
            _typedModuleProvider = typedModuleProvider;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        public IEnumerable<TBadge> GetBadges()
        {
            throw new NotImplementedException();
        }

        public Task<IDictionary<string, IEnumerable<TBadge>>> GetCategorizedBadgesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
