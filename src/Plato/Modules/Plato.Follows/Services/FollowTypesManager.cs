using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Follows.Models;

namespace Plato.Follows.Services
{

    public class FollowTypesManager : IFollowTypesManager
    {

        private IEnumerable<IFollowType> _followTypes;

        private readonly IEnumerable<IFollowTypeProvider> _providers;
        private readonly ILogger<FollowTypesManager> _logger;

        public FollowTypesManager(
            IEnumerable<IFollowTypeProvider> providers,
            ILogger<FollowTypesManager> logger)
        {
            _providers = providers;
            _logger = logger;
        }

        public IEnumerable<IFollowType> GetFollowTypes()
        {

            if (_followTypes == null)
            {
                var notificationTypes = new List<IFollowType>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        notificationTypes.AddRange(provider.GetFollowTypes());
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"An exception occurred within the follow type provider '{provider.GetType()}'. Please review your follow type provider and try again. {e.Message}");
                        throw;
                    }
                }

                _followTypes = notificationTypes;
            }

            return _followTypes;

        }

    }
}
