using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Plato.Stars.Models;

namespace Plato.Stars.Services
{

    public class StarTypesManager : IStarTypesManager
    {

        private IEnumerable<IStarType> _followTypes;

        private readonly IEnumerable<IStarTypeProvider> _providers;
        private readonly ILogger<StarTypesManager> _logger;

        public StarTypesManager(
            IEnumerable<IStarTypeProvider> providers,
            ILogger<StarTypesManager> logger)
        {
            _providers = providers;
            _logger = logger;
        }

        public IEnumerable<IStarType> GetFollowTypes()
        {

            if (_followTypes == null)
            {
                var notificationTypes = new List<IStarType>();
                foreach (var provider in _providers)
                {
                    try
                    {
                        notificationTypes.AddRange(provider.GetFollowTypes());
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e,
                            $"An exception occurred within the star type provider '{provider.GetType()}'. Please review your follow type provider and try again. {e.Message}");
                        throw;
                    }
                }

                _followTypes = notificationTypes;
            }

            return _followTypes;

        }

    }
}
